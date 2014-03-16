using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace StorageUnitTest
{
    [TestClass]
    public class StorageBridgeTest
    {
        private UserAcc _user1;
        private UserAcc _user2;
        private UserAcc _user3;
        private Mock<IStorageConnection> _mock;

        [TestMethod]
        public void AddTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(true);
            var sud = new StorageBridge(_mock.Object);
            sud.Add(new UserAcc());
        }
        
        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void AddFailedTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(false);
            var sud = new StorageBridge(_mock.Object);
            sud.Add(new UserAcc());
        }

        [TestMethod]
        [ExpectedException(typeof(InternalDbException))]
        public void AddFailedIdNotSetTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(true);
            _mock.Setup(foo => foo.Add(It.IsAny<UserAcc>()));
            var sud = new StorageBridge(_mock.Object);
            sud.Add(new UserAcc());
        }

        [TestMethod]
        public void UpdateTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(true);
            var sud = new StorageBridge(_mock.Object);
            sud.Update(_user1);
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void UpdateFailedTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(false);
            var sud = new StorageBridge(_mock.Object);
            sud.Update(_user1);
        }

        [TestMethod]
        public void DeleteTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(true);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete(_user1);
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void DeleteFailedTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(false);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete(_user1);
        }

        [TestMethod]
        public void DeleteByIdTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(true);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete<UserAcc>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void DeleteByIdFailedTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(false);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete<UserAcc>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteByIdNotFoundFailedTest()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(false);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete<UserAcc>(4);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            var sud = new StorageBridge(_mock.Object);
            Assert.AreEqual(_user1,sud.Get<UserAcc>(1));
            Assert.AreEqual(_user2, sud.Get<UserAcc>(2));
            Assert.AreEqual(_user3, sud.Get<UserAcc>(3));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetByIdWrongIdTest()
        {
            var sud = new StorageBridge(_mock.Object);
            sud.Get<UserAcc>(4);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var sud = new StorageBridge(_mock.Object);
            var e = sud.Get<UserAcc>();
            Assert.AreEqual(_user1,e.Single(t => t.Id == 1));
            Assert.AreEqual(_user2, e.Single(t => t.Id == 2));
            Assert.AreEqual(_user3, e.Single(t => t.Id == 3));
        }

        [TestMethod]
        public void GetAllEmptyStorageTest()
        {
            _mock.Setup(foo => foo.Get<UserAcc>()).Returns(new List<UserAcc>().AsQueryable);
            var sud = new StorageBridge(_mock.Object);
            Assert.AreEqual(0, sud.Get<UserAcc>().Count());
        }

        [TestInitialize]
        public void Setup()
        {
            _user1 = new UserAcc
            {
                Id = 1,
                Password = "1",
                Username = "U1"
            };

            _user2 = new UserAcc
            {
                Id = 2,
                Password = "2",
                Username = "U2"
            };

            _user3 = new UserAcc
            {
                Id = 3,
                Password = "3",
                Username = "U3"
            };

            _mock = new Mock<IStorageConnection>();
            var users = new HashSet<UserAcc> {_user1, _user2, _user3};
            _mock.Setup(foo => foo.Get<UserAcc>()).Returns(users.AsQueryable);
            _mock.Setup(foo => foo.Add(It.IsAny<UserAcc>())).Callback<UserAcc>((user) => user.Id = 1);
            _mock.Setup(foo => foo.Delete(_user1));
            _mock.Setup(foo => foo.Delete(_user2));
            _mock.Setup(foo => foo.Delete(_user3));
            _mock.Setup(foo => foo.Update(_user1));
            _mock.Setup(foo => foo.Update(_user2));
            _mock.Setup(foo => foo.Update(_user3));
        }
    }
}
