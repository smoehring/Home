using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Smoehring.Home.Data.SqlDatabase
{
    public static class Extensions
    {
        public static void MigrateDatabase<T>(this IHost host) where T : DbContext
            {
                var serviceScope = host.Services.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;

                var dbContext = serviceProvider.GetRequiredService<T>();
                
                dbContext.Database.Migrate();

                return;
            }
    }
}
