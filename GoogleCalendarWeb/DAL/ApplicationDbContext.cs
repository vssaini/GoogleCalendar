using System.Data.Entity;
using ZenegyCalendar.DAL;

namespace ZenegyCalendar.Infrastructure
{
    internal class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<ApplicationDbContext>());
        }

        public DbSet<GoogleAuthItem> GoogleAuthItems { get; set; }
    }
}
