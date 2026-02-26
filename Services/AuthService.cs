using Booking_System.Data;
using Booking_System.DTOs.Auth;
using Booking_System.Helpers;
using Booking_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Booking_System.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(UserEmail => UserEmail.Email == dto.Email))
                throw new Exception("البريد الإلكتروني مستخدم بالفعل");

            bool isFirstUser = !await _context.Users.AnyAsync();

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = isFirstUser ? "Admin" : "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(UserEmail => UserEmail.Email == dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                throw new Exception("البريد الإلكتروني أو كلمة المرور غير صحيحة");

            return _jwtHelper.GenerateToken(user);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
