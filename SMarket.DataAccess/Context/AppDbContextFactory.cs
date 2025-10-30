using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SMarket.DataAccess.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Tìm tới file appsettings.json trong project Presentation
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../SMarket.Presentation");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("SMarket.DataAccess").UseVector());

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
