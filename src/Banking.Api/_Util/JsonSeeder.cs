using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

using Banking.Core.Entities;
using Banking.Persistence.EFCore;
using Microsoft.Extensions.Configuration;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore;

namespace Banking.Api.Extensions
{
    
    public static class JsonSeeder
    {

        /// <summary>
        /// Extension for seeding the database with entities represented as json
        /// </summary>
        /// <param name="app"></param>
        /// <param name="jsonFilepath">the full path to the json seed file</param>
        /// <param name="config"></param>
        public static void EnsureDatabaseSeeded(this IApplicationBuilder app, string jsonFilepath)
        {
            EnsureDatabaseSeeded(app.ApplicationServices, jsonFilepath);
        }
        
        public static void EnsureDatabaseSeeded(IServiceProvider provider, string jsonFilepath, bool dropDatabase = false)
        {
            var seedJsonText = System.IO.File.ReadAllText(jsonFilepath);
            var seedEntities = JsonSerializer.Deserialize<SeedModel>(seedJsonText, new JsonSerializerOptions()
            {
                 AllowTrailingCommas = true,
                 PropertyNameCaseInsensitive = true, 
            });
            using (var scope = provider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (dropDatabase)
                {
                    db.Database.EnsureDeleted();
                }

                db.Database.Migrate();

                if (!db.Institutions.Any())
                {
                    db.AddRange(seedEntities.Institutions);
                    db.AddRange(seedEntities.Members);
                    db.SaveChanges();
                }
               
            }
        }
    }

    /// <summary>
    /// Wrapper for the json container used to hold seed data
    /// </summary>
    public class SeedModel
    {
        public List<Institution> Institutions { get; set; }
        public List<Member> Members { get; set; }
    }


}
