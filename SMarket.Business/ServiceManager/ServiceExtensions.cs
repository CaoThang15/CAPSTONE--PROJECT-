using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMarket.DataAccess.Context;

namespace SMarket.Business.ServiceManager
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SMarket.DataAccess")));
        }

        public static void ConfigureBusinessServices(this IServiceCollection services)
        {
            // Add your business services here
            // Example: services.AddScoped<IUserService, UserService>();
        }
    }
}
