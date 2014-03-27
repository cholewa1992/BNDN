using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using BusinessLayerTest;
using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BusinessLogicTests
{
    [TestClass]
    public class AuthLogicTest: BaseTest
    {
        private IStorageBridge _dbStorage;

        #region Setup
        [TestInitialize]
        public void Setup()
        {
            SetupDbStorageMock();
        }

        private void SetupDbStorageMock()
        {
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(foo => foo.Add(It.IsAny<Entity>())).Verifiable();
            dbMoq.Setup(foo => foo.Update(It.IsAny<Entity>())).Verifiable();
            var accessRights = SetupAccessRights();
            dbMoq.Setup(foo => foo.Get<AccessRight>()).Returns(accessRights.AsQueryable());
            _dbStorage = dbMoq.Object;
        }

        private HashSet<AccessRight> SetupAccessRights()
        {
            var ar1 = new AccessRight //User 1 has bought Entity 1
            {
                Id = 1,
                Expiration = new DateTime(2014, 12, 24, 23, 59, 59),
                EntityId = 1, 
                UserId = 1,
                AccessRightTypeId = 2
            };
            var ar2 = new AccessRight //User 2 owns Entity 1
            {
                Id = 2,
                Expiration = null,
                EntityId = 1,
                UserId = 2,
                AccessRightTypeId = 1
            };
            var ar3 = new AccessRight //User 1 has bought Entity 1 before, but it expired last christmas
            {
                Id = 3,
                Expiration = new DateTime(2013, 12, 24, 23, 59, 59),
                EntityId = 1,
                UserId = 1,
                AccessRightTypeId = 2
            };
            var ar4 = new AccessRight //User 3 has bought Entity 1 but has no expiration
            {
                Id = 4,
                Expiration = null,
                EntityId = 1,
                UserId = 3,
                AccessRightTypeId = 2
            };
            var ar5 = new AccessRight //User 3 has bought Entity 2 but it has expired
            {
                Id = 5,
                Expiration = new DateTime(2013, 12, 31, 00, 00, 00),
                EntityId = 2,
                UserId = 3,
                AccessRightTypeId = 2
            };
            var ar6 = new AccessRight //User 2 has bought Entity 2
            {
                Id = 6,
                Expiration = new DateTime(2015, 01, 01, 00, 00, 00),
                EntityId = 2,
                UserId = 2,
                AccessRightTypeId = 2
            };
            return new HashSet<AccessRight> { ar1, ar2, ar3, ar4, ar5, ar6 };
        }
        #endregion

        #region GetExpirationDate
        /* userId < 1
         * mediaItemId < 1
         * userId not found
         * mediaItemId not found
         * userId and mediaItemId exists but no access
         * user is buyer
         * user owns item
         * expired item
         * user is buyer but no expiration
         * buyer with multiple ARs (only one is not expired) 
         */

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetExpirationDate_InvalidUserId()
        {
            var authLogic = new AuthLogic(_dbStorage);
            authLogic.GetExpirationDate(0, 1);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetExpirationDate_InvalidMediaItemId()
        {
            var authLogic = new AuthLogic(_dbStorage);
            authLogic.GetExpirationDate(1, 0);
        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void GetExpirationDate_UserIdNotFound()
        {
            var authLogic = new AuthLogic(_dbStorage);
            authLogic.GetExpirationDate(999, 1);

        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void GetExpirationDate_MediaItemIdNotFound()
        {
            var authLogic = new AuthLogic(_dbStorage);
            authLogic.GetExpirationDate(1, 999);
        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void GetExpirationDate_NoAccessRight()
        {
            var authLogic = new AuthLogic(_dbStorage);
            authLogic.GetExpirationDate(1, 2);
        }

        [TestMethod]
        public void GetExpirationDate_BuyerWithOneExpiration()
        {
            var authLogic = new AuthLogic(_dbStorage);
            var expectedExpiration = new DateTime(2015, 01, 01, 00, 00, 00);
            var actualExpiration = authLogic.GetExpirationDate(2, 2);
            Assert.AreEqual(expectedExpiration, actualExpiration);
        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void GetExpirationDate_Owner()
        {
            var authLogic = new AuthLogic(_dbStorage);
            authLogic.GetExpirationDate(2, 1);
        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void GetExpirationDate_BuyerButExpired()
        {
            var authLogic = new AuthLogic(_dbStorage);
            authLogic.GetExpirationDate(3, 2);
        }

        [TestMethod]
        public void GetExpirationDate_BuyerWithoutExpiration()
        {
            var authLogic = new AuthLogic(_dbStorage);
            var expectedExpiration = new DateTime(9999, 12, 31);
            var actualExpiration = authLogic.GetExpirationDate(3, 1);
            Assert.AreEqual(expectedExpiration, actualExpiration);
        }

        [TestMethod]
        public void GetExpirationDate_BuyerWithMultipleExpirations()
        {
            var authLogic = new AuthLogic(_dbStorage);
            var expectedExpiration = new DateTime(2014, 12, 24, 23, 59, 59);
            var actualExpiration = authLogic.GetExpirationDate(1, 1);
            Assert.AreEqual(expectedExpiration, actualExpiration);
        }

        #endregion


    }
}
