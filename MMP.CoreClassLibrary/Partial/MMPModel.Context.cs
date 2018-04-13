using MMP.CoreClassLibrary.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMPModel
{

    public partial class MMPEntities : DbContext
    {

        #region Méthodes partielles

        partial void OnContextCreated()
        {
            //Database.SetInitializer<MMPEntities>(new CreateDatabaseIfNotExists<MMPEntities>());
            //Database.SetInitializer<MMPEntities>(new DropCreateDatabaseIfModelChanges<MMPEntities>());
            //Database.SetInitializer<MMPEntities>(new DropCreateDatabaseAlways<MMPEntities>());

            //Let's use our custom Initializer
            Database.SetInitializer<MMPEntities>(new MMPDatabaseInitializer());
#if DEBUG
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
        }

        partial void OnContextCreated(string nameOrConnectionString)
        {
            //Let's use our custom Initializer
            Database.SetInitializer<MMPEntities>(new MMPDatabaseInitializer(nameOrConnectionString));
#if DEBUG
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
        }

        #endregion

        #region Méthodes surchargées

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Configuration des Entités du modèle

            modelBuilder.Properties().Where(p => p.Name == "Id").Configure(p => p.IsKey());

            //User
            modelBuilder.Entity<User>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<User>().HasOptional(p => p.Email);
            //modelBuilder.Entity<User>().HasRequired(p => p.Object).WithMany(p => p.Objects).WillCascadeOnDelete(true);

            #endregion

            base.OnModelCreating(modelBuilder);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Exception exception = GetInnerException(ex);

                if (exception is SqlException sqlException)
                {
                    SqlError sqlError;
                    string constraintName;

                    if (sqlException.Errors.OfType<SqlError>().Any(se => se.Number == 2627))//PK/UK
                    {
                        sqlError = sqlException.Errors.OfType<SqlError>().First(se => se.Number == 2627);//PK/UK

                        if (sqlError.Message.Contains("'PK_"))
                        {
                            constraintName = sqlError.Message.Substring(sqlError.Message.IndexOf("'PK_") + 1);
                        }
                        else if (sqlError.Message.Contains("'UK_"))
                        {
                            constraintName = sqlError.Message.Substring(sqlError.Message.IndexOf("'UK_") + 1);
                        }
                        else
                        {
                            throw new Exception(sqlError.Message);
                        }
                    }
                    else if (sqlException.Errors.OfType<SqlError>().Any(se => se.Number == 2601))//UI
                    {
                        sqlError = sqlException.Errors.OfType<SqlError>().First(se => se.Number == 2601);//UI

                        if (sqlError.Message.Contains("'UI_"))
                        {
                            constraintName = sqlError.Message.Substring(sqlError.Message.IndexOf("'UI_") + 1);
                        }
                        else
                        {
                            throw new Exception(sqlError.Message);//SQR: custom message for not managed exceptions?
                        }
                    }
                    else
                    {
                        throw GetInnerException(ex);
                    }

                    constraintName = constraintName.Substring(0, constraintName.IndexOf("'"));
                    string msg = null;//manage msg with constraintName
                    throw new Exception(string.IsNullOrEmpty(msg) ? sqlError.Message : msg);
                }
                else
                {
                    throw GetInnerException(ex);
                }
            }
            catch (Exception e)
            {
                throw GetInnerException(e);
            }
        }

        #endregion

        private Exception GetInnerException(Exception ex)
        {
            if (ex == null) return null;

            var innerEx = ex;
            while (innerEx.InnerException != null)
            {
                innerEx = innerEx.InnerException;
            }
            return innerEx;
        }
    }
}
