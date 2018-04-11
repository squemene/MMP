using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsLibrary;

namespace MMP.CoreClassLibrary.DB
{
    internal class MMPDatabaseInitializer : AutoMigrateDatabaseInitializer<MMPEntities, Migrations.Configuration>
    {
        public MMPDatabaseInitializer() : base()
        {
        }
        public MMPDatabaseInitializer(string connectionStringName) : base(connectionStringName)
        {
        }
    }

    internal abstract class AutoMigrateDatabaseInitializer<TContext, TConfiguration> : IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TConfiguration : DbMigrationsConfiguration<TContext>, new()
    {

        private readonly DbMigrationsConfiguration _configuration;

        public AutoMigrateDatabaseInitializer()
        {
            _configuration = new TConfiguration();
        }

        public AutoMigrateDatabaseInitializer(string connectionStringName)
        {
            System.Diagnostics.Contracts.Contract.Requires(!string.IsNullOrEmpty(connectionStringName), "connectionStringName");
            _configuration = new TConfiguration
            {
                TargetDatabase = new DbConnectionInfo(connectionStringName)
            };
        }

        void IDatabaseInitializer<TContext>.InitializeDatabase(TContext context)
        {
            System.Diagnostics.Contracts.Contract.Requires(context != null, "context");

            Logger.Debug("Initializing Database");

            bool dbExists = context.Database.Exists();
            bool compatible;
            try
            {
                compatible = context.Database.CompatibleWithModel(throwIfNoMetadata: true);
            }
            catch (Exception e)
            {
                Logger.Info("The database is not not compatible : " + e.Message);
                compatible = false;
            }
            var migrator = new DbMigrator(_configuration);

            if (migrator.GetPendingMigrations().Any())
            {
                Logger.Debug("There are some pending migrations : " + migrator.GetPendingMigrations().Count());

                var pendingMigrations = migrator.GetPendingMigrations();

                //run migrations
                foreach (string mig in pendingMigrations)
                {
                    Logger.Debug("Updating Migration:" + mig);
                    //execute the migration
                    migrator.Update(mig);
                    Logger.Info("Updated Migration:" + mig);
                }

                Logger.Debug("Migrations successfully performed");
            }
        }
    }
}
