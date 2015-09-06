using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace LoadFileData.Web
{
    public class Migration<T> : DbMigrationsConfiguration<T> where T : DbContext
    {
        public Migration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(T context)
        { }

    }
}