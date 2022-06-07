using System.IO;
using AutoLot.Dal.EfStructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace AutoLot.Tests.Helpers
{
    public static class TestHelpers
    {
        public static IConfiguration GetConfiguration()
        {
            var configuration = new ConfigurationBuilder();
            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddJsonFile("appsettings.json", true, true);

            return configuration.Build();
        }

        public static ApplicationDbContext GetDbContext(IConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("AutoLot");

            optionsBuilder.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        public static ApplicationDbContext GetSecondContext(ApplicationDbContext oldContext,
            IDbContextTransaction transaction)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(oldContext.Database.GetConnectionString(),
                options => options.EnableRetryOnFailure());

            var context = new ApplicationDbContext(optionsBuilder.Options);

            context.Database.UseTransaction(transaction.GetDbTransaction());

            return context;
        }
    }
}
