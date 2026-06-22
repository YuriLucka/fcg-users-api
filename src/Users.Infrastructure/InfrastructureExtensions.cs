using Users.Application.Interfaces;
using Users.Domain.Interfaces;
using Users.Infrastructure.Data;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                ));

            // Repositórios
            services.AddScoped<IUserRepository, UserRepository>();

            // Segurança
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
