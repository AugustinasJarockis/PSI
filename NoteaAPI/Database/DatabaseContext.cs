using Microsoft.EntityFrameworkCore;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Models.FileTree;

namespace NoteaAPI.Database
{ 
        public class DatabaseContext : DbContext
        {
            public DbSet<ConspectModel> Conspects { get; set; }
            public DbSet<UserModel> Users { get; set; }
            public DbSet<UserConspectsModel> UserConspects { get; set; }
            public DbSet<TreeNodeModel> FileStructure { get; set; }
            public DbSet<FolderModel> Folders { get; set; }
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

