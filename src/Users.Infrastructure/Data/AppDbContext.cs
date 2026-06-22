using Users.Domain.Entities;
using Users.Domain.Enums;
using Users.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todos os mappings do assembly automaticamente
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public static async Task SeedAsync(AppDbContext context)
        {
            await SeedUsersAsync(context);
        }

        private static async Task SeedUsersAsync(AppDbContext context)
        {
            // Admin user
            if (!await context.Users.AnyAsync(u => u.Email == new Email("admin@fcg.com")))
            {
                User.ValidateRawPassword("Admin@123");
                var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                var admin = new User(
                    name: "Administrador",
                    email: "admin@fcg.com",
                    passwordHash: adminPasswordHash,
                    role: UserRole.Admin
                );
                await context.Users.AddAsync(admin);
            }

            // User accounts
            var userInfos = new[]
            {
                new { Name = "Yuri", Email = "yuri@fcg.com" },
                new { Name = "Rafael", Email = "rafael@fcg.com" },
                new { Name = "Pedro", Email = "pedro@fcg.com" },
                new { Name = "Gustavo", Email = "gustavo@fcg.com" },
                new { Name = "Carlos", Email = "carlos@fcg.com" }
            };
            User.ValidateRawPassword("Fiap@123");
            var userPasswordHash = BCrypt.Net.BCrypt.HashPassword("Fiap@123");
            foreach (var info in userInfos)
            {
                if (!await context.Users.AnyAsync(u => u.Email == new Email(info.Email)))
                {
                    var user = new User(
                        name: info.Name,
                        email: info.Email,
                        passwordHash: userPasswordHash,
                        role: UserRole.User
                    );
                    await context.Users.AddAsync(user);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
