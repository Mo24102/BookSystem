
using Booking_System.Data;
using Booking_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking_System.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> GetUsersAsync()
        {
            return await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role
                })
                .ToListAsync<object>();
        }

        public async Task MakeAdminAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.Role = "Admin";
            await _context.SaveChangesAsync();
        }

        // ── جديد: تعديل مستخدم ──
        public async Task UpdateUserAsync(int userId, User dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                    throw new Exception("Email already exists");
                user.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrEmpty(dto.Role))
                user.Role = dto.Role;

            await _context.SaveChangesAsync();
        }

        // ── جديد: حذف مستخدم ──
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // تحقق لو في عملاء مرتبطين - اختياري: لو عايز تمسح العملاء معاه أو ترفض الحذف
            // هنا بس نمسح المستخدم

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
