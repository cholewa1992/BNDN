﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StorageTest
{
    /// <summary>
    /// Unit test for testing EF implmentation
    /// </summary>
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    [TestClass]
    public class EFUnitTest
    {
        #region Setup
        [TestInitialize]
        public void Setup()
        {
            using (var db = new RentIt08Entities())
            {
                db.Database.ExecuteSqlCommand(Properties.);
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Asbjørn', '12345678')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Thomas', '87654321')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Mathias', '43218765')");

                db.Database.ExecuteSqlCommand("INSERT INTO Client (Name, Token) VALUES ('ArtShare', 'abcdefg12345')");

                db.Database.ExecuteSqlCommand("INSERT INTO EntityType (Type) VALUES ('Book')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityType (Type) VALUES ('Movie')");

                //db.Database.ExecuteSqlCommand("INSERT INTO Entity (FilePath, ClientId, TypeId) VALUES ('C:/path', 1, 1)");
                //db.Database.ExecuteSqlCommand("INSERT INTO Entity (FilePath, ClientId, TypeId) VALUES ('C:/path', 1, 2)");

                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES ('Title')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES ('Description')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES ('Language')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES ('YearOfPublication')");

                //db.Database.ExecuteSqlCommand("INSERT INTO EntityInfo (Data, EntityId, EntityTypeId) VALUES ('Test data', 2, 1)");
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            using (var db = new RentIt08Entities())
            {
                //db.Database.ExecuteSqlCommand();
            }
        }
        #endregion
        #region GetTests
        [TestMethod]
        public void UnitTest_EF_GetUsers()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                Assert.AreEqual(3, db.Get<UserAcc>().Count());
            }
        }

        [TestMethod]
        public void UnitTest_EF_GetUserUsingWhereTest()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                Assert.AreEqual(1, db.Get<UserAcc>().Count(t => t.Username == "Asbjørn"));
            }
        }
        #endregion
        #region Add Tests

        [TestMethod]
        public void UnitTest_EF_AddUserTest()
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
        public void UnitTest_EF_AddUserTwiceTest()
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
        public void UnitTest_EF_AddEmptyUserTest()

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
        public void UnitTest_EF_AddUserWithOnlyUsername()
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
        public void UnitTest_EF_AddUserWithOnlyPassword()
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
        public void UnitTest_EF_UpdateUserTest()
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
        public void UnitTest_EF_UpdateUserIdTest()
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
        public void UnitTest_EF_DeleteUserTest()
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
        public void UnitTest_EF_DeleteUserNotInDbTest()
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
