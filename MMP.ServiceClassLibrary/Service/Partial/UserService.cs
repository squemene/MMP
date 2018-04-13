using Mehdime.Entity;
using MMP.ServiceClassLibrary.MockModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMPModel.Service
{
    public partial class UserService
    {

        #region UserCreation

        public void CreateUser(MockUser userToCreate)
        {
            if (userToCreate == null)
                throw new ArgumentNullException("userToCreate");

            //userToCreate.Validate();

            /*
			 * Typical usage of DbContextScope for a read-write business transaction. 
			 * It's as simple as it looks.
			 */
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                //-- Build domain model
                var user = new User()
                {
                    Id = userToCreate.Id,
                    Name = userToCreate.Name,
                    Email = userToCreate.Email,
                    WelcomeEmailSent = false,
                    CreatedOn = DateTime.UtcNow
                };

                //-- Persist
                var addedUser = UserRepository.Add(user);
                int saveCount = dbContextScope.SaveChanges();

                //return addedUser;
            }
        }

        public User DeleteUser(Guid userId)
        {

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<MMPModel.MMPEntities>();
                var user = dbContext.Users.Find(userId);
                //var a = new User();

                if (user == null)
                    throw new ArgumentException(String.Format("Invalid value provided for userId: [{0}]. Couldn't find a user with this ID.", userId));

                dbContext.Users.Remove(user);

                int saveCount = dbContextScope.SaveChanges();

                return user;
            }
        }

        public void CreateListOfUsers(params MockUser[] usersToCreate)
        {
            /*
			 * Example of DbContextScope nesting in action. 
			 * 
			 * We already have a service method - CreateUser() - that knows how to create a new user
			 * and implements all the business rules around the creation of a new user 
			 * (e.g. validation, initialization, sending notifications to other domain model objects...).
			 * 
			 * So we'll just call it in a loop to create the list of new users we've 
			 * been asked to create.
			 * 
			 * Of course, since this is a business logic service method, we are making 
			 * an implicit guarantee to whoever is calling us that the changes we make to 
			 * the system will be either committed or rolled-back in an atomic manner. 
			 * I.e. either all the users we've been asked to create will get persisted
			 * or none of them will. It would be disastrous to have a partial failure here
			 * and end up with some users but not all having been created.
			 * 
			 * DbContextScope makes this trivial to implement. 
			 * 
			 * The inner DbContextScope instance that the CreateUser() method creates
			 * will join our top-level scope. This ensures that the same DbContext instance is
			 * going to be used throughout this business transaction.
			 * 
			 */

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var toCreate in usersToCreate)
                {
                    CreateUser(toCreate);
                }

                // All the changes will get persisted here
                int saveCount = dbContextScope.SaveChanges();
            }
        }

        public void CreateListOfUsersWithIntentionalFailure(params MockUser[] usersToCreate)
        {
            /*
			 * Here, we'll verify that inner DbContextScopes really join the parent scope and 
			 * don't persist their changes until the parent scope completes successfully. 
			 */

            var firstUser = true;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var toCreate in usersToCreate)
                {
                    if (firstUser)
                    {
                        CreateUser(toCreate);
                        Console.WriteLine("Successfully created a new User named '{0}'.", toCreate.Name);
                        firstUser = false;
                    }
                    else
                    {
                        // OK. So we've successfully persisted one user.
                        // We're going to simulate a failure when attempting to 
                        // persist the second user and see what ends up getting 
                        // persisted in the DB.
                        throw new Exception(String.Format("Oh no! An error occurred when attempting to create user named '{0}' in our database.", toCreate.Name));
                    }
                }

                dbContextScope.SaveChanges();
            }
        }

        #endregion

        #region UserQuery

        public User GetUser(Guid userId)
        {
            /*
			 * An example of using DbContextScope for read-only queries. 
			 * Here, we access the Entity Framework DbContext directly from 
			 * the business logic service class.
			 * 
			 * Calling SaveChanges() is not necessary here (and in fact not 
			 * possible) since we created a read-only scope.
			 */
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<MMPModel.MMPEntities>();
                var user = dbContext.Users.Find(userId);

                if (user == null)
                    throw new ArgumentException(String.Format("Invalid value provided for userId: [{0}]. Couldn't find a user with this ID.", userId));

                return user;
            }
        }

        public IEnumerable<User> GetUsers(params Guid[] userIds)
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<MMPModel.MMPEntities>();
                return dbContext.Users.Where(u => userIds.Contains(u.Id)).ToList();
            }
        }

        public User GetUserViaRepository(Guid userId)
        {
            /*
			 * Same as GetUsers() but using a repository layer instead of accessing the 
			 * EF DbContext directly.
			 * 
			 * Note how we don't have to worry about knowing what type of DbContext the 
			 * repository will need, about creating the DbContext instance or about passing
			 * DbContext instances around. 
			 * 
			 * The DbContextScope will take care of creating the necessary DbContext instances
			 * and making them available as ambient contexts for our repository layer to use.
			 * It will also guarantee that only one instance of any given DbContext type exists
			 * within its scope ensuring that all persistent entities managed within that scope
			 * are attached to the same DbContext. 
			 */
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var user = UserRepository.Find(userId);

                if (user == null)
                    throw new ArgumentException(String.Format("Invalid value provided for userId: [{0}]. Couldn't find a user with this ID.", userId));

                return user;
            }
        }

        public async Task<IList<User>> GetTwoUsersAsync(Guid userId1, Guid userId2)
        {
            /*
			 * A very contrived example of ambient DbContextScope within an async flow.
			 * 
			 * Note that the ConfigureAwait(false) calls here aren't strictly necessary 
			 * and are unrelated to DbContextScope. You can remove them if you want and 
			 * the code will run in the same way. It is however good practice to configure
			 * all your awaitables in library code to not continue 
			 * on the captured synchronization context. It avoids having to pay the overhead 
			 * of capturing the sync context and running the task continuation on it when 
			 * library code doesn't need that context. If also helps prevent potential deadlocks 
			 * if the upstream code has been poorly written and blocks on async tasks. 
			 * 
			 * "Library code" is any code in layers under the presentation tier. Typically any code
			 * other that code in ASP.NET MVC / WebApi controllers or Window Form / WPF forms.
			 * 
			 * See http://blogs.msdn.com/b/pfxteam/archive/2012/04/13/10293638.aspx for 
			 * more details.
			 */

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var user1 = await UserRepository.FindAsync(userId1).ConfigureAwait(false);

                // We're now in the continuation of the first async task. This is most
                // likely executing in a thread from the ThreadPool, i.e. in a different
                // thread that the one where we created our DbContextScope. Our ambient
                // DbContextScope is still available here however, which allows the call 
                // below to succeed.

                var user2 = await UserRepository.FindAsync(userId2).ConfigureAwait(false);

                // In other words, DbContextScope works with async execution flow as you'd expect: 
                // It Just Works.  

                return new List<User> { user1, user2 }.Where(u => u != null).ToList();
            }
        }

        public User GetUserUncommitted(Guid userId)
        {
            /*
			 * An example of explicit database transaction. 
			 * 
			 * Read the comment for CreateReadOnlyWithTransaction() before using this overload
			 * as there are gotchas when doing this!
			 */
            using (_dbContextScopeFactory.CreateReadOnlyWithTransaction(IsolationLevel.ReadUncommitted))
            {
                return UserRepository.Find(userId);
            }
        }

        #endregion

        #region UserCreditScore

        public void UpdateCreditScoreForAllUsers()
        {
            /*
			 * Demo of DbContextScope + parallel programming.
			 */

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                //-- Get all users
                var dbContext = dbContextScope.DbContexts.Get<MMPModel.MMPEntities>();
                var userIds = dbContext.Users.Select(u => u.Id).ToList();

                Console.WriteLine("Found {0} users in the database. Will calculate and store their credit scores in parallel.", userIds.Count);

                //-- Calculate and store the credit score of each user
                // We're going to imagine that calculating a credit score of a user takes some time. 
                // So we'll do it in parallel.

                // You MUST call SuppressAmbientContext() when kicking off a parallel execution flow 
                // within a DbContextScope. Otherwise, this DbContextScope will remain the ambient scope
                // in the parallel flows of execution, potentially leading to multiple threads
                // accessing the same DbContext instance.
                using (_dbContextScopeFactory.SuppressAmbientContext())
                {
                    Parallel.ForEach(userIds, UpdateCreditScore);
                }

                // Note: SaveChanges() isn't going to do anything in this instance since all the changes
                // were actually made and saved in separate DbContextScopes created in separate threads.
                int saveCount = dbContextScope.SaveChanges();
            }
        }

        public void UpdateCreditScore(Guid userId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<MMPModel.MMPEntities>();
                var user = dbContext.Users.Find(userId);
                if (user == null)
                    throw new ArgumentException(String.Format("Invalid userId provided: {0}. Couldn't find a User with this ID.", userId));

                // Simulate the calculation of a credit score taking some time
                var random = new Random(Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(random.Next(300, 1000));

                user.CreditScore = random.Next(1, 100);
                int saveCount = dbContextScope.SaveChanges();
            }
        }

        #endregion

        #region UserEmail

        public void SendWelcomeEmail(Guid userId)
        {
            /*
			 * Demo of forcing the creation of a new DbContextScope
			 * to ensure that changes made to the model in this service 
			 * method are persisted even if that method happens to get
			 * called within the scope of a wider business transaction
			 * that eventually fails for any reason.
			 * 
			 * This is an advanced feature that should be used as rarely 
			 * as possible (and ideally, never).
			 */

            // We're going to send a welcome email to the provided user
            // (if one hasn't been sent already). Once sent, we'll update
            // that User entity in our DB to record that its Welcome email
            // has been sent.

            // Emails can't be rolled-back. Once they're sent, they're sent. 
            // So once the email has been sent successfully, we absolutely 
            // must persist this fact in our DB. Even if that method is called
            // by another busines logic service method as part of a wider 
            // business transaction and even if that parent business transaction
            // ends up failing for any reason, we still must ensure that
            // we have recorded the fact that the Welcome email has been sent.
            // Otherwise, we would risk spamming our users with repeated Welcome
            // emails. 

            // Force the creation of a new DbContextScope so that the changes we make here are
            // guaranteed to get persisted regardless of what happens after this method has completed.
            using (var dbContextScope = _dbContextScopeFactory.Create(DbContextScopeOption.ForceCreateNew))
            {
                var dbContext = dbContextScope.DbContexts.Get<MMPModel.MMPEntities>();
                var user = dbContext.Users.Find(userId);

                if (user == null)
                    throw new ArgumentException(String.Format("Invalid userId provided: {0}. Couldn't find a User with this ID.", userId));

                if (!user.WelcomeEmailSent)
                {
                    SendEmail(user.Email);
                    user.WelcomeEmailSent = true;
                }

                int saveCount = dbContextScope.SaveChanges();

                // When you force the creation of a new DbContextScope, you must force the parent
                // scope (if any) to reload the entities you've modified here. Otherwise, the method calling
                // you might not be able to see the changes you made here.
                dbContextScope.RefreshEntitiesInParentScope(new List<User> { user });
            }
        }

        private void SendEmail(string emailAddress)
        {
            // Send the email synchronously. Throw if any error occurs.
            // [...]
        }

        #endregion
    }
}
