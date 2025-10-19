using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NordenAPI.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NordenDbContext>
    {
        public NordenDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NordenDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=NordenDB;Username=postgres;Password=password");

            return new NordenDbContext(optionsBuilder.Options);
        }
    }
}
