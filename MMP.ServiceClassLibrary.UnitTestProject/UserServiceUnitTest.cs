using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMP.ServiceClassLibrary.MockModel;
using MMPModel.Service;
using ToolsLibrary;

namespace MMP.ServiceClassLibrary.UnitTestProject
{
    [TestClass]
    public class UserServiceUnitTest
    {
        [TestClass]
        public class UserServiceTest
        {

            //global for the test run
            private static TestContext testContext;

            private ServiceFactory svcProvider;
            private UserService userSvc;

            #region Initialisation and cleanup

            /// <summary>
            /// Execute once before the test-suite
            /// </summary>
            [ClassInitialize]
            public static void ClassInitialize(TestContext context)
            {
                testContext = context;
            }

            [TestInitialize]
            public void TestInitialize()
            {
                svcProvider = new ServiceFactory();
                userSvc = svcProvider.Get<UserService>();
            }

            [TestCleanup]
            public void TestCleanup()
            {
                svcProvider = null;
                userSvc = null;
            }

            [ClassCleanup]
            public static void ClassCleanup()
            {
            }

            #endregion

            [TestMethod]
            public void TypicalReadAndWrite()
            {
                //-- Demo of typical usage for read and writes

                var marySpec = new MockUser("Mary", "mary@example.com");

                userSvc.CreateUser(marySpec);

                var mary = userSvc.GetUser(marySpec.Id);

                Assert.AreEqual("Mary", mary.Name, "Le nom enregistré doit être égale à Mary");
            }

            [TestMethod]
            public void NestedDbContextScopes()
            {
                //-- Demo of nested DbContextScopes

                // Creating 2 new users called John and Jeanne in an atomic transaction...
                var johnSpec = new MockUser("John", "john@example.com");
                var jeanneSpec = new MockUser("Jeanne", "jeanne@example.com");

                userSvc.CreateListOfUsers(johnSpec, jeanneSpec);

                // Trying to retrieve our newly created users from the data store...
                var createdUsers = userSvc.GetUsers(johnSpec.Id, jeanneSpec.Id);

                Assert.AreEqual(2, createdUsers.Count(), "Le nombre d'utilisateur enregistrés doit être égale à 2");
            }

            [TestMethod]
            public void NestedDbContextScopesException()
            {
                //-- Demo of nested DbContextScopes in the face of an exception.

                // If any of the provided users failed to get persisted, none should get persisted. 
                var julieSpec = new MockUser("Julie", "julie@example.com");
                var marcSpec = new MockUser("Marc", "marc@example.com");
                try
                {
                    userSvc.CreateListOfUsersWithIntentionalFailure(julieSpec, marcSpec);
                    Assert.Fail("L'enregistrement des utilisateurs ne doit pas marcher...");
                }
                catch (Exception e)
                {
                    Assert.IsNotNull(e, "Une exception aurait du être levée");
                }

                //Trying to retrieve our newly created users from the data store...
                var maybeCreatedUsers = userSvc.GetUsers(julieSpec.Id, marcSpec.Id);
                Assert.AreEqual(0, maybeCreatedUsers.Count(), "Le nombre d'utilisateur enregistrés doit être égale à 0");
            }

        }
    }
}
