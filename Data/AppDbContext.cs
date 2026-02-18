
using Booking_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();

            // علاقة المستخدم بالعملاء
            modelBuilder.Entity<Client>()
                .HasOne(client => client.CreatedByUser)
                .WithMany(user => user.Clients)
                .HasForeignKey(client => client.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .HasIndex(expense => expense.Category);

            modelBuilder.Entity<Expense>()
                .HasIndex(expense => expense.ExpenseDate);

            modelBuilder.Entity<Expense>()
                .HasOne(expense => expense.CreatedByUser)
                .WithMany()
                .HasForeignKey(expense => expense.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}