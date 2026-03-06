using Booking_System.Data;
using Booking_System.DTOs.Client;
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

        public async Task<int> AddClientAsync(ClientCreateDto dto, int userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

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

                ActualCost = dto.ActualCost,
                TotalDue = dto.TotalDue,
                PaidAmount = dto.PaidAmount,

                PaymentStatus = dto.TotalDue <= 0
                    ? "غير مدفوع"
                    : dto.PaidAmount >= dto.TotalDue
                        ? "مدفوع كامل"
                        : "دفع جزئي",

                LastPaymentDate = dto.PaidAmount > 0 ? DateTime.UtcNow : null,
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
            string? serviceType = null)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 5, 100);

            var query = _context.Clients
                .Include(client => client.CreatedByUser)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(client => client.CreatedByUserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(client =>
                    EF.Functions.Like(client.ClientName, $"%{search}%") ||
                    EF.Functions.Like(client.Phone, $"%{search}%") ||
                    EF.Functions.Like(client.NationalId, $"%{search}%"));
            }

            if (!string.IsNullOrWhiteSpace(serviceType))
            {
                query = query.Where(client => client.ServiceType == serviceType.Trim());
            }

            int totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(client => client.CreatedAt)
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

                    ActualCost = isAdmin ? Client.ActualCost : 0m,
                    TotalDue = isAdmin ? Client.TotalDue : 0m,
                    PaidAmount = isAdmin ? Client.PaidAmount : 0m,
                    RemainingAmount = isAdmin ? (Client.TotalDue - Client.PaidAmount) : 0m,
                    PaymentStatus = isAdmin ? Client.PaymentStatus : "غير مصرح",
                    Profit = isAdmin ? (Client.PaidAmount - Client.ActualCost) : 0m,

                    NumberOfInstallments = isAdmin ? (Client.NumberOfInstallments ?? 0) : 0,
                    PaidInstallments = isAdmin ? (Client.PaidInstallments ?? 0) : 0,
                    LastPaymentDate = isAdmin ? Client.LastPaymentDate : null,
                    FinalDueDate = isAdmin ? Client.FinalDueDate : null,
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

        public async Task UpdateClientAsync(int id, ClientUpdateDto dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                throw new Exception("العميل غير موجود");

            if (!string.IsNullOrWhiteSpace(dto.ClientName)) client.ClientName = dto.ClientName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Phone)) client.Phone = dto.Phone.Trim();
            if (!string.IsNullOrWhiteSpace(dto.NationalId)) client.NationalId = dto.NationalId.Trim();
            if (!string.IsNullOrWhiteSpace(dto.ServiceType)) client.ServiceType = dto.ServiceType.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Notes)) client.Notes = dto.Notes.Trim();

            if (dto.TotalDue.HasValue) client.TotalDue = dto.TotalDue.Value;
            if (dto.ActualCost.HasValue) client.ActualCost = dto.ActualCost.Value;
            if (dto.NumberOfInstallments.HasValue) client.NumberOfInstallments = dto.NumberOfInstallments.Value;
            if (dto.FinalDueDate.HasValue) client.FinalDueDate = dto.FinalDueDate.Value;

            if (dto.AdditionalPayment.HasValue && dto.AdditionalPayment.Value > 0)
            {
                client.PaidAmount += dto.AdditionalPayment.Value;
                client.LastPaymentDate = DateTime.UtcNow;

                if (client.TotalDue > 0 && (client.NumberOfInstallments ?? 0) > 0)
                {
                    decimal installmentValue = client.TotalDue / client.NumberOfInstallments.Value;
                    client.PaidInstallments = (int)(client.PaidAmount / installmentValue);
                    if (client.PaidInstallments > client.NumberOfInstallments)
                        client.PaidInstallments = client.NumberOfInstallments;
                }

                client.PaymentStatus = client.PaidAmount >= client.TotalDue
                    ? "مدفوع كامل"
                    : (client.PaidAmount > 0 ? "دفع جزئي" : "غير مدفوع");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
