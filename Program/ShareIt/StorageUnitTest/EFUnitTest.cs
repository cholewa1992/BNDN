using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StorageUnitTest
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
                db.Database.ExecuteSqlCommand(Properties.Resources.datamodel);
                db.Database.ExecuteSqlCommand(Properties.Resources.testdata);
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            using (var db = new RentIt08Entities())
            {
                db.Database.ExecuteSqlCommand(Properties.Resources.datamodel);
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
        public void UnitTest_EF_GetUserUsingWhere()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                Assert.AreEqual(1, db.Get<UserAcc>().Count(t => t.Username == "Jacob"));
            }
        }
        #endregion
        #region Add Tests

        [TestMethod]
        public void UnitTest_EF_AddUser()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = new UserAcc
                {
                    Username = "Asbjørn",
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
        public void UnitTest_EF_AddUserTwice()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = new UserAcc
                {
                    Username = "Asbjørn",
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
        public void UnitTest_EF_AddEmptyUser()

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
                    Username = "Asbjørn"
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
        public void UnitTest_EF_UpdateUser()
        {
            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Mathias' AND Password = '1234'").Single();
                Assert.AreNotEqual(expected, result);
            }

            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = db.Get<UserAcc>().Single(t => t.Username == "Mathias");
                user.Password = "1234";
                db.Update(user);
                db.SaveChanges();
            }

            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Mathias' AND Password = '1234'").Single();
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnitTest_EF_UpdateUserId()
        {
            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = db.Get<UserAcc>().Single(t => t.Username == "Mathias");
                user.Id = 1234;
                db.Update(user);
                db.SaveChanges();
            }
        }
        #endregion
        #region Delete Tests
        [TestMethod]
        public void UnitTest_EF_DeleteUser()
        {
            using (var db = new RentIt08Entities())
            {
                const int expected = 1;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Mathias'").Single();
                Assert.AreEqual(expected, result);
            }

            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = db.Get<UserAcc>().Single(t => t.Username == "Mathias");
                db.Delete(user);
                db.SaveChanges();
            }

            using (var db = new RentIt08Entities())
            {
                const int expected = 0;
                var result = db.Database.SqlQuery<int>("SELECT Count(*) FROM UserAcc WHERE Username = 'Mathias' AND Password = '4321'").Single();
                Assert.AreEqual(expected, result);
            }
        }
        #endregion
        #region Auto increment tests


        [TestMethod]
        public void UnitTest_EF_UserAccAutoIncrement()
        {
            int maxId;
            using (var db = new RentIt08Entities())
            {
                maxId = db.Database.SqlQuery<int>("SELECT max(id) FROM UserAcc").Single();
            }

            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var user = new UserAcc
                {
                    Username = "Asbjørn",
                    Password = "testtesttest"
                };

                db.Add(user);
                db.SaveChanges();
            }

            using (var db = new RentIt08Entities())
            {
                var result = db.Database.SqlQuery<int>("SELECT id FROM UserAcc WHERE Username = 'Asbjørn'").Single();
                Assert.AreEqual(maxId + 1, result);
            }
        }

        [TestMethod]
          public void UnitTest_EF_MediaItemAutoIncrement()
          {
              int maxId;
              using (var db = new RentIt08Entities())
              {
                  maxId = db.Database.SqlQuery<int>("SELECT max(id) FROM Entity").Single();
              }            

              using (var db = new EfStorageConnection<RentIt08Entities>())
              {
                  var mediaItem = new Entity
                  {
                      FilePath = "C:/path/test",
                      ClientId = 1,
                      TypeId = 1
                  };
  
                  db.Add(mediaItem);
                  db.SaveChanges();
              }
  
              using (var db = new RentIt08Entities())
              {
                  var result = db.Database.SqlQuery<int>("SELECT id FROM Entity WHERE FilePath = 'C:/path/test'").Single();
                  Assert.AreEqual(maxId + 1, result);
              }
          }

        [TestMethod]
        public void UnitTest_EF_MediaItemInformationAutoIncrement()
        {
            int maxId;
            using (var db = new RentIt08Entities())
            {
                maxId = db.Database.SqlQuery<int>("SELECT max(id) FROM EntityInfo").Single();
            } 

            using (var db = new EfStorageConnection<RentIt08Entities>())
            {
                var mediaItemInformation = new EntityInfo
                {
                    Data = "Test",
                    EntityId = 1,
                    EntityInfoTypeId = 4
                };

                db.Add(mediaItemInformation);
                db.SaveChanges();
            }

            using (var db = new RentIt08Entities())
            {
                var result = db.Database.SqlQuery<int>("SELECT id FROM EntityInfo WHERE EntityInfoTypeId = 4").Single();
                Assert.AreEqual(maxId + 1, result);
            }
        }
        #endregion
    }
}
