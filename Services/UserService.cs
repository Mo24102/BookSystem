using Booking_System.Data;
using Booking_System.DTOs.Auth;
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

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(user => new User
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                })
                .ToListAsync();
        }

        public async Task MakeAdminAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("المستخدم مش موجود");

            user.Role = "Admin";
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(int userId, RegisterDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("المستخدم غير موجود");

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (await _context.Users.AnyAsync(user => user.Email == dto.Email && user.Id != userId))
                    throw new Exception("البريد الإلكتروني مستخدم بالفعل");
                user.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrEmpty(dto.Role))
                user.Role = dto.Role;

            await _context.SaveChangesAsync();
        }


        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
