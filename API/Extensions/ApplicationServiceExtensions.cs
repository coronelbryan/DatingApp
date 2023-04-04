using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();

            services.AddDbContext<DataContext>(
                options =>
                {
                    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                    // options.UseSqlite("Data source=datingapp.db");                    
                }
            );

            return services;
        }
    }
}