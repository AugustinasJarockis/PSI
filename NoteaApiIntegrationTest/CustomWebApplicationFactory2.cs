using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NoteaAPI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoteaAPI.Models.UserModels;

namespace NoteaApiIntegrationTest
{
    public class CustomWebApplicationFactory2<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryIntegrationTests");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                SeedTestData(dbContext);

                dbContext.Database.EnsureCreated();
            });
        }
        private void SeedTestData(DatabaseContext dbContext)
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = 55, Username = "user1", Password = "Abc123123", Email = "user1@example.com" },
                new UserModel { Id = 66, Username = "user2", Password = "Abc123123", Email = "user2@example.com" },
            };

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }
    }
}
