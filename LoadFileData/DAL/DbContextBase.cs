using System.Data.Entity;
using LoadFileData.DAL.Models;

namespace LoadFileData.DAL
{
    public class DbContextBase : DbContext
    {
        public DbSet<FileSource> FileSources { get; set; }
        public DbSet<Error> Errors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Properties()
                .Where(pi => 
                    (pi.PropertyType == typeof (decimal)) || 
                    (pi.PropertyType == typeof (decimal?)))
                .Configure(c => c.HasPrecision(32, 8));
            base.OnModelCreating(modelBuilder);
        }
    }
}
