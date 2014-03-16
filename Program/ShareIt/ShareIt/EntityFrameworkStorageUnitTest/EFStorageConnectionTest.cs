using System.Linq;
using EntityFrameworkStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;

namespace EntityFrameworkStorageUnitTest
{
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// Morten Rosenmeier (morr@itu.dk)
    /// </author>
    [TestClass]
    public class EFStorageConnectionTest
    {
        [TestMethod]
        public void AddToContextTest()
        {
            using (var ef = new EFConnectionFactory<MockFakeImdbContext>().CreateConnection())
            {
                var user = new UserAcc {Email = "jbec@itu.dk", Password = "1234" };
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Email == user.Email));

                ef.Add(user);
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Email == user.Email));

                Assert.IsTrue(ef.SaveChanges());
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Email == user.Email));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalDbException))]
        public void AddInvalidDataToContextTest()
        {
            using (var ef = new EFConnectionFactory<MockFakeImdbContext>().CreateConnection())
            {
                var user = new UserAcc {Email = "jbec@itu.dk"};
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Email == user.Email));
                ef.Add(user);
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Email == user.Email));
                Assert.IsTrue(ef.SaveChanges());
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Email == user.Email));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalDbException))]
        public void AddWithIdSetTest()
        {
            using (var ef = new EFConnectionFactory<MockFakeImdbContext>().CreateConnection())
            {
                var user = new UserAcc { Id = 1 };
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Email == user.Email));
                ef.Add(user);
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Email == user.Email));
                Assert.IsTrue(ef.SaveChanges());
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Email == user.Email));
            }
        }

        [TestMethod]
        public void UpdateEntityInContextTest()
        {
            using (var ef = new EFConnectionFactory<MockFakeImdbContext>().CreateConnection())
            {
                var user = new UserAcc { Email = "jbec@itu.dk", Password = "1234" };
                ef.Add(user);
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec@itu.dk"));
                ef.SaveChanges();
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec@itu.dk"));

                user.Email = "jbec1@itu.dk";
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec@itu.dk"));
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec1@itu.dk"));

                ef.Update(user);
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec@itu.dk"));
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec1@itu.dk"));

                ef.SaveChanges();
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec@itu.dk"));
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec1@itu.dk"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalDbException))]
        public void UpdateEntityNotInContextTest()
        {
            using (var ef = new EFConnectionFactory<MockFakeImdbContext>().CreateConnection())
            {
                var user = new UserAcc { Email = "jbec@itu.dk", Password = "1234" };
                user.Email = "jbec1@itu.dk";
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == user.Email));
                ef.Update(user);
                ef.SaveChanges();
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Id == user.Id && t.Email == "jbec1@itu.dk"));
            }
        }

        [TestMethod]
        public void DeleteEntityInContextTest()
        {
            using (var ef = new EFConnectionFactory<MockFakeImdbContext>().CreateConnection())
            {
                var user = new UserAcc { Email = "jbec@itu.dk", Password = "1234" };
                ef.Add(user);
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id));

                ef.SaveChanges();
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Id == user.Id));

                ef.Delete(user);
                Assert.IsTrue(ef.Get<UserAcc>().Any(t => t.Id == user.Id));

                ef.SaveChanges();
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id));
                
            }
        }
        [TestMethod]
        [ExpectedException(typeof(InternalDbException))]
        public void DeleteEntityNotInContextTest()
        {
            using (var ef = new EFConnectionFactory<MockFakeImdbContext>().CreateConnection())
            {
                var user = new UserAcc { Email = "jbec@itu.dk", Password = "1234" };
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id));
                ef.Delete(user);
                ef.SaveChanges();
                Assert.IsFalse(ef.Get<UserAcc>().Any(t => t.Id == user.Id));
            }
        }
    }
}
