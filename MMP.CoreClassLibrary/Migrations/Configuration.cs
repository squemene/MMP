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

            context.Users.AddOrUpdate(
              p => p.Name,
              new User { Id = Guid.NewGuid(), Name = "Andrew Peters", Email = "andrew.peters@mail.com", CreatedOn = DateTime.Now },
              new User { Id = Guid.NewGuid(), Name = "Brice Lambson", Email = "brice.lambson@mail.com", CreatedOn = DateTime.Now },
              new User { Id = Guid.NewGuid(), Name = "Rowan Miller", Email = "rowan.miller@mail.com", CreatedOn = DateTime.Now }
            );

        }
    }
}
