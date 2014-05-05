using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace StorageUnitTest
{
    /// <summary>
    /// Class for testing Storage bridge logic
    /// </summary>
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    [TestClass]
    public class StorageBridgeTest
    {
        #region Fields
        private UserAcc _user1;
        private UserAcc _user2;
        private UserAcc _user3;
        private Mock<IStorageConnection> _mock;
        #endregion
        #region Setup
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
            var users = new HashSet<UserAcc> { _user1, _user2, _user3 };
            _mock.Setup(foo => foo.Get<UserAcc>()).Returns(users.AsQueryable);
            _mock.Setup(foo => foo.Add(It.IsAny<UserAcc>())).Callback<UserAcc>((user) => user.Id = 1);
            _mock.Setup(foo => foo.Delete(_user1));
            _mock.Setup(foo => foo.Delete(_user2));
            _mock.Setup(foo => foo.Delete(_user3));
            _mock.Setup(foo => foo.Update(_user1));
            _mock.Setup(foo => foo.Update(_user2));
            _mock.Setup(foo => foo.Update(_user3));
        }
        #endregion
        #region Add tests
        [TestMethod]
        public void UnitTest_StorageBridge_Add()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(1);
            var sud = new StorageBridge(_mock.Object);
            sud.Add(new UserAcc());
        }
        
        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void UnitTest_StorageBridge_AddFailed()
        {
            _mock.Setup(foo => foo.SaveChanges()).Throws<ChangesWasNotSavedException>();
            var sud = new StorageBridge(_mock.Object);
            sud.Add(new UserAcc());
        }

        [TestMethod]
        [ExpectedException(typeof(InternalDbException))]
        public void UnitTest_StorageBridge_AddFailedIdNotSet()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(1);
            _mock.Setup(foo => foo.Add(It.IsAny<UserAcc>()));
            var sud = new StorageBridge(_mock.Object);
            sud.Add(new UserAcc());
        }
        #endregion
        #region Update tests
        [TestMethod]
        public void UnitTest_StorageBridge_Update()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(1);
            var sud = new StorageBridge(_mock.Object);
            sud.Update(_user1);
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void UnitTest_StorageBridge_UpdateFailed()
        {
            _mock.Setup(foo => foo.SaveChanges()).Throws<ChangesWasNotSavedException>();
            var sud = new StorageBridge(_mock.Object);
            sud.Update(_user1);
        }
        #endregion
        #region Delete tests
        [TestMethod]
        public void UnitTest_StorageBridge_Delete()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(1);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete(_user1);
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void UnitTest_StorageBridge_DeleteFailed()
        {
            _mock.Setup(foo => foo.SaveChanges()).Throws<ChangesWasNotSavedException>();
            var sud = new StorageBridge(_mock.Object);
            sud.Delete(_user1);
        }

        [TestMethod]
        public void UnitTest_StorageBridge_DeleteById()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(1);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete<UserAcc>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ChangesWasNotSavedException))]
        public void UnitTest_StorageBridge_DeleteByIdFailed()
        {
            _mock.Setup(foo => foo.SaveChanges()).Throws<ChangesWasNotSavedException>();
            var sud = new StorageBridge(_mock.Object);
            sud.Delete<UserAcc>(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnitTest_StorageBridge_DeleteByIdNotFoundFailed()
        {
            _mock.Setup(foo => foo.SaveChanges()).Returns(0);
            var sud = new StorageBridge(_mock.Object);
            sud.Delete<UserAcc>(4);
        }
        #endregion
        #region Get tests
        [TestMethod]
        public void UnitTest_StorageBridge_GetById()
        {
            var sud = new StorageBridge(_mock.Object);
            Assert.AreEqual(_user1,sud.Get<UserAcc>(1));
            Assert.AreEqual(_user2, sud.Get<UserAcc>(2));
            Assert.AreEqual(_user3, sud.Get<UserAcc>(3));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnitTest_StorageBridge_GetByIdWrongId()
        {
            var sud = new StorageBridge(_mock.Object);
            sud.Get<UserAcc>(4);
        }

        [TestMethod]
        public void UnitTest_StorageBridge_GetAll()
        {
            var sud = new StorageBridge(_mock.Object);
            var e = sud.Get<UserAcc>();
            Assert.AreEqual(_user1,e.Single(t => t.Id == 1));
            Assert.AreEqual(_user2, e.Single(t => t.Id == 2));
            Assert.AreEqual(_user3, e.Single(t => t.Id == 3));
        }

        [TestMethod]
        public void UnitTest_StorageBridge_GetAllEmptyStorage()
        {
            _mock.Setup(foo => foo.Get<UserAcc>()).Returns(new List<UserAcc>().AsQueryable);
            var sud = new StorageBridge(_mock.Object);
            Assert.AreEqual(0, sud.Get<UserAcc>().Count());
        }
        #endregion

    }
}
