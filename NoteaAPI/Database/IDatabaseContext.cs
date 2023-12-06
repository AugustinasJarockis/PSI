using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Models.FileTree;

namespace NoteaAPI.Database
{
    public interface IDatabaseContext
    {
        public DbSet<ConspectModel> Conspects { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserConspectsModel> UserConspects { get; set; }
        public DbSet<TreeNodeModel> FileStructure { get; set; }
        public DbSet<FolderModel> Folders { get; set; }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        public int SaveChanges();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
