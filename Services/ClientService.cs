using Booking_System.Data;
using Booking_System.DTOs;
using Booking_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking_System.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddClientAsync(ClientResponseItemDto dto, int userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.ClientName))
                throw new ArgumentException("اسم العميل مطلوب");

            if (string.IsNullOrWhiteSpace(dto.Phone))
                throw new ArgumentException("رقم التليفون مطلوب");

            var client = new Client
            {
                ClientName = dto.ClientName.Trim(),
                Phone = dto.Phone.Trim(),
                NationalId = dto.NationalId?.Trim(),
                ServiceType = dto.ServiceType?.Trim(),
                Notes = dto.Notes?.Trim(),
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,

                // الحقول المالية
                ActualCost = dto.ActualCost,
                TotalDue = dto.TotalDue,
                PaidAmount = dto.PaidAmount,

                // حساب الحالة تلقائيًا
                PaymentStatus = dto.TotalDue <= 0
                    ? "غير مدفوع"
                    : dto.PaidAmount >= dto.TotalDue
                        ? "مدفوع كامل"
                        : "دفع جزئي",

                LastPaymentDate = dto.PaidAmount > 0 ? DateTime.UtcNow : null,

                // اختياري - إذا كنت تستخدمه
                NumberOfInstallments = dto.NumberOfInstallments,
                FinalDueDate = dto.FinalDueDate
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client.Id;
        }

        public async Task<object> GetClientsAsync(
            int userId,
            bool isAdmin,
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? serviceType = null 
            )
        {
            // Sanitize and validate pagination
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 5, 100); // reasonable limits

            var query = _context.Clients
                .Include(c => c.CreatedByUser)
                .AsQueryable();

            if (!isAdmin)
               {
                   query = query.Where(c => c.CreatedByUserId == userId);
               }

            // Search - better to use EF.Functions.Like for database compatibility & Arabic support
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(c =>
                    EF.Functions.Like(c.ClientName, $"%{search}%") ||
                    EF.Functions.Like(c.Phone, $"%{search}%") ||
                    EF.Functions.Like(c.NationalId, $"%{search}%")
                );
            }

            // Filter by service type
            if (!string.IsNullOrWhiteSpace(serviceType))
            {
                query = query.Where(c => c.ServiceType == serviceType.Trim());
            }
            int totalCount = await query.CountAsync();

            // Fetch paginated data
            var data = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(Client => new ClientResponseItemDto
                {
                    Id = Client.Id,
                    ClientName = Client.ClientName,
                    Phone = Client.Phone,
                    NationalId = Client.NationalId,
                    ServiceType = Client.ServiceType,
                    Notes = Client.Notes,
                    CreatedBy = Client.CreatedByUser.FullName,
                    CreatedAt = Client.CreatedAt,

                    // الحقول المالية - تظهر للأدمن فقط
                    ActualCost = isAdmin ? Client.ActualCost : 0m,
                    TotalDue = isAdmin ? Client.TotalDue : 0m,
                    PaidAmount = isAdmin ? Client.PaidAmount : 0m,
                    RemainingAmount = isAdmin ? (Client.TotalDue - Client.PaidAmount) : 0m,
                    PaymentStatus = isAdmin ? Client.PaymentStatus : "غير مصرح",
                    Profit = isAdmin ? (Client.PaidAmount - Client.ActualCost) : 0m,

                    // اختياري
                    NumberOfInstallments = isAdmin ? (Client.NumberOfInstallments ?? 0) : 0,
                    PaidInstallments = isAdmin ? (Client.PaidInstallments ?? 0) : 0,
                    LastPaymentDate = isAdmin ? Client.LastPaymentDate : null,
                    FinalDueDate = isAdmin ? Client.FinalDueDate : null,
                    AdditionalPayment = 0
                })
                .ToListAsync();

            return new ClientListResponseDto
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = data
            };
        }

        public async Task UpdateClientAsync(int id, ClientResponseItemDto dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                throw new Exception("Client not found");

            // تحديث الحقول النصية
            if (!string.IsNullOrWhiteSpace(dto.ClientName)) client.ClientName = dto.ClientName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Phone)) client.Phone = dto.Phone.Trim();
            if (!string.IsNullOrWhiteSpace(dto.NationalId)) client.NationalId = dto.NationalId.Trim();
            if (!string.IsNullOrWhiteSpace(dto.ServiceType)) client.ServiceType = dto.ServiceType.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Notes)) client.Notes = dto.Notes.Trim();

            // تحديث القيم الأساسية لو مبعوتة
            if (dto.TotalDue != 0) client.TotalDue = dto.TotalDue;
            if (dto.ActualCost != 0) client.ActualCost = dto.ActualCost;
            if (dto.NumberOfInstallments.HasValue) client.NumberOfInstallments = dto.NumberOfInstallments.Value;
            if (dto.FinalDueDate.HasValue) client.FinalDueDate = dto.FinalDueDate.Value;

            // --- المنطق الذكي لتحصيل الدفعات وحساب الأقساط ---
            if (dto.AdditionalPayment.HasValue && dto.AdditionalPayment.Value > 0)
            {
                // 1. إضافة المبلغ الجديد للمدفوع سابقاً
                client.PaidAmount += dto.AdditionalPayment.Value;
                client.LastPaymentDate = DateTime.UtcNow;

                // 2. حساب عدد الأقساط المدفوعة بناءً على إجمالي المدفوع
                if (client.TotalDue > 0 && (client.NumberOfInstallments ?? 0) > 0)
                {
                    // قيمة القسط الواحد = الإجمالي / عدد الأقساط
                    decimal installmentValue = client.TotalDue / client.NumberOfInstallments.Value;

                    // عدد الأقساط المدفوعة = إجمالي اللي دفعه / قيمة القسط الواحد
                    // استخدمنا (int) عشان يقرب لأقل رقم صحيح (يعني لو دفع قسط ونصف، يتحسب قسط واحد كامل)
                    client.PaidInstallments = (int)(client.PaidAmount / installmentValue);

                    // تأكيد إن الأقساط المدفوعة م تزيدش عن الإجمالي
                    if (client.PaidInstallments > client.NumberOfInstallments)
                        client.PaidInstallments = client.NumberOfInstallments;
                }

                // 3. تحديث حالة الدفع
                client.PaymentStatus = client.PaidAmount >= client.TotalDue
                    ? "مدفوع كامل"
                    : (client.PaidAmount > 0 ? "دفع جزئي" : "غير مدفوع");
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return false; // Not found → we return false instead of throwing
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return true;
        }
        }
}
