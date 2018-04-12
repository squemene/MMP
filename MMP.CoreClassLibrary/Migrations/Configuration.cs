namespace MMP.CoreClassLibrary.Migrations
{
    using MMPModel;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MMPModel.MMPEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MMPModel.MMPEntities context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            context.People.AddOrUpdate(
              p => p.FullName,
              new Person { FullName = "Andrew Peters", Email = "andrew.peters@mail.com", CreatedOn = DateTime.Now },
              new Person { FullName = "Brice Lambson", Email = "brice.lambson@mail.com", CreatedOn = DateTime.Now },
              new Person { FullName = "Rowan Miller", Email = "rowan.miller@mail.com", CreatedOn = DateTime.Now }
            );

        }
    }
}
