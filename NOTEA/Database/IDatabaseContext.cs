using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;

namespace NOTEA.Database
{
    public interface IDatabaseContext
    {
        public DbSet<ConspectModel> Conspects { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserConspectsModel> UserConspects { get; set; }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        public int SaveChanges();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
