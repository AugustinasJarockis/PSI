using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;

namespace NOTEA.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ConspectModel> Conspects { get; set; }

        //public DbSet<UserModel> Users { get; set; }     

        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }
    }
}
