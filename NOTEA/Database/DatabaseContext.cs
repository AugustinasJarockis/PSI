using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;

namespace NOTEA.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ConspectModel> Conspects { get; set; }
        public DbSet<UserModel> Users { get; set; }     
        public DbSet<UserConspectsModel> UserConspects { get; set; }
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserModel>()
                .HasIndex(u => u.Username)
                .IsUnique();
            builder.Entity<UserConspectsModel>()
                .HasKey(s => s.No);
        }
    }
}
