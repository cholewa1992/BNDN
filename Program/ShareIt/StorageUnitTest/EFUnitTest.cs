using System;
using System.Linq;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StorageUnitTest
{
    [TestClass]
    public class EFUnitTest
    {
        #region Setup
        [TestInitialize]
        public void Setup()
        {
            using (var db = new RentIt08Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM UserAcc;");
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Asbjørn', '12345678')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Thomas', '87654321')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Mathias', '43218765')");

            }
        }

        [TestCleanup]
        public void TearDown()
        {
            using (var db = new RentIt08Entities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM UserAcc;");
            }
        }
        #endregion
        #region GetTests
        [TestMethod]
        public void EF_GetUsersTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                Assert.AreEqual(3, db.Get<UserAcc>().Count());
            }
        }

        [TestMethod]
        public void EF_GetUserUsingWhereTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                Assert.AreEqual(1, db.Get<UserAcc>().Count(t => t.Username == "Asbjørn"));
            }
        }
        #endregion
        #region Add Tests

        [TestMethod]
        public void EF_AddUserTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = new UserAcc
                {
                    Username = "Jacob",
                    Password = "testtesttest"
                };

                db.Add(user);
                db.SaveChanges();
            }

            using (var db = new RentIt08Entities())
            {
                const int expected = 4;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc").Single();
                Assert.AreEqual(expected, result);
            }
        }


        [TestMethod]
        [ExpectedException(typeof(InternalDbException))]
        public void EF_AddUserTwiceTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = new UserAcc
                {
                    Username = "Jacob",
                    Password = "testtesttest"
                };

                db.Add(user);
                db.SaveChanges();
                db.Add(user);
                db.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void EF_AddEmptyUserTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = new UserAcc();
                db.Add(user);
                db.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void EF_AddUserWithOnlyUsernameTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                db.Add(new UserAcc
                {
                    Username = "Jacob"
                });
                db.SaveChanges();
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void EF_AddUserWithOnlyPasswordTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                db.Add(new UserAcc
                {
                    Password = "1234"
                });
                db.SaveChanges();
            }
        }
        #endregion
        #region Update Tests
        [TestMethod]
        public void EF_UpdateUserTest()
        {
            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Asbjørn' AND Password = '1234'").Single();
                Assert.AreNotEqual(expected, result);
            }

            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = db.Get<UserAcc>().Single(t => t.Username == "Asbjørn");
                user.Password = "1234";
                db.Update(user);
                db.SaveChanges();
            }

            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Asbjørn' AND Password = '1234'").Single();
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EF_UpdateUserIdTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = db.Get<UserAcc>().Single(t => t.Username == "Asbjørn");
                user.Id = 1234;
                db.Update(user);
                db.SaveChanges();
            }
        }
        #endregion
        #region Delete Tests
        [TestMethod]
        public void EF_DeleteUserTest()
        {
            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Asbjørn'").Single();
                Assert.AreEqual(expected, result);
            }

            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = db.Get<UserAcc>().Single(t => t.Username == "Asbjørn");
                db.Delete(user);
                db.SaveChanges();
            }

            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Asbjørn' AND Password = '1234'").Single();
                Assert.AreNotEqual(expected, result);
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void EF_DeleteUserNotInDbTest()
        {
            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Jacob'").Single();
                Assert.AreNotEqual(expected, result);
            }

            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = new UserAcc()
                {
                    Id = 1,
                    Username = "Jacob",
                    Password = "1234"
                };
                db.Delete(user);
                db.SaveChanges();
            }
        }
        #endregion
    }
}
