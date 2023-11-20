using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;

namespace NOTEA.Database
{
    public abstract class IDatabaseContext : DbContext
    {
        public IDatabaseContext()
        {

        }

        public IDatabaseContext(DbContextOptions<IDatabaseContext> options)
            : base(options)
        {
        }
        public virtual DbSet<ConspectModel> Conspects { get; set; }
        public virtual DbSet<UserModel> Users { get; set; }
        public virtual DbSet<UserConspectsModel> UserConspects { get; set; }
        
    }
}
