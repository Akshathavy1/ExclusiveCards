using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExclusiveCard.Data.Context
{
    public class ExclusiveCardFactory : IDesignTimeDbContextFactory<ExclusiveContext>
    {
        public ExclusiveContext CreateDbContext(string [] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ExclusiveContext>();

            var connectionString = configuration.GetConnectionString("exclusive");

            builder
                .UseSqlServer(connectionString);
            

            return new ExclusiveContext(builder.Options);
            //DbContextOptionsBuilder<ExclusiveContext> optionsBuilder = new DbContextOptionsBuilder<ExclusiveContext>();
            ////optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["name=exclusive"].ConnectionString);
            //optionsBuilder.UseSqlServer("name=exclusive");
            //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            //return new ExclusiveContext(optionsBuilder.Options);
        }
    }
}
