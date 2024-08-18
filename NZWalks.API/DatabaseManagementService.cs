using Microsoft.EntityFrameworkCore;
using NZWalks.Infrastructure.DataContext;

namespace NZWalks.API
{
    public class DatabaseManagementService
    {
        public static void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<NZWalksDbContext>()?.Database.Migrate();
                serviceScope.ServiceProvider.GetService<NZWalksAuthDbContext>()?.Database.Migrate();

                //var dbContext = serviceScope.ServiceProvider.GetRequiredService<NZWalksDbContext>();
                //var authDbContext = serviceScope.ServiceProvider.GetRequiredService<NZWalksAuthDbContext>();

                //if (!dbContext.Database.GetAppliedMigrations().Any())
                //{
                //    dbContext.Database.Migrate();
                //}

                //if (!authDbContext.Database.GetAppliedMigrations().Any())
                //{
                //    authDbContext.Database.Migrate();
                //}
            }
        }
    }
}
