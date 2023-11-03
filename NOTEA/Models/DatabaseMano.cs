using Microsoft.EntityFrameworkCore;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;

namespace NOTEA.Models
{
    public class DatabaseMano : DbContext
    {
        public DbSet<ConspectModel> Conspects { get; set; }

        //public DbSet<UserModel> Users { get; set; }     

        public DatabaseMano(DbContextOptions options) : base(options)
        {
            
        }
    }
}
