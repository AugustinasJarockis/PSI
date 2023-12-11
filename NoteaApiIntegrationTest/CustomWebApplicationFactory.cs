using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoteaAPI.Database;
using NoteaAPI.Models.UserModels;

namespace NoteaApiIntegrationTest
{
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
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

                // Call the method to seed the database
                SeedTestData(dbContext);

                dbContext.Database.EnsureCreated();
            });
        }

        private void SeedTestData(DatabaseContext dbContext)
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = 1, Username = "user1", Password = "password1", Email = "user1@example.com" },
                new UserModel { Id = 2, Username = "user2", Password = "password2", Email = "user2@example.com" },
            };

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }
    }
}
