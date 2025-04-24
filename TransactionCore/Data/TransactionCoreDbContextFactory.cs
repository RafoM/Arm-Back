using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace TransactionCore.Data
{
    public class TransactionCoreDbContextFactory : IDesignTimeDbContextFactory<TransactionCoreDbContext>
    {
        public TransactionCoreDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TransactionCoreDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            return new TransactionCoreDbContext(optionsBuilder.Options);
        }
    }
}
