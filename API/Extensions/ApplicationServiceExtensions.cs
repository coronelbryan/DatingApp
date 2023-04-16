using API.Data;
using API.Interfaces;
using API.Services;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();

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