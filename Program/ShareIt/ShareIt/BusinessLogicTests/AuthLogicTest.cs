
﻿using System;
using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Management.Instrumentation;
﻿using BusinessLayerTest;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
﻿using AccessRightType = DataAccessLayer.AccessRightType;

namespace BusinessLogicTests
{
    [TestClass]
    public class AuthLogicTest: BaseTest
    {


        private IAuthInternalLogic al;


        [TestInitialize]
        public void Initiate()
        {
             var testData = new HashSet<IEntityDto>
             {
                 new Client()
                 {
                     Id = 1,
                     Name = "testClient",
                     Token = "testToken"
                 },
                 new Client()
                 {
                     Name = "testClient2",
                     Token = "testToken2"
                 },
                 new UserAcc()
                 {
                     Id = 1,
                     Username = "username",
                     Password = "password"
                 },
                 new UserAcc()
                 {
                     Username = "username2",
                     Password = "password2"
                 },
                 new AccessRight()
                 {
                     EntityId = 1,
                     UserId = 1,
                     Expiration = null,
                     AccessRightTypeId = 1
                 },
                 new AccessRight()
                 {
                     EntityId = 2,
                     UserId = 1,
                     Expiration = new DateTime(2050,1,1),
                     AccessRightTypeId = 1
                 },
                 new AccessRight()
                 {
                     EntityId = 3,
                     UserId = 1,
                     Expiration = new DateTime(2010,1,1),
                     AccessRightTypeId = 1
                 },
                 new AccessRightType()
                 {
                     Id = 1,
                     Name = "Owner"
                 }
             };

            var bridgeStub = new StorageBridgeStub(testData);

            al = new AuthLogic(bridgeStub);

            Setup();
        }

        [TestMethod]
        public void ClientTokenIsWithSystem()
        {
            Assert.AreEqual(1, al.CheckClientToken("testToken"));
        }

        [TestMethod]
        public void ClientTokenIsNotWithSystem()
        {
            Assert.AreEqual(-1, al.CheckClientToken("noToken"));
        }

        [TestMethod]
        public void CheckUserAccessIsOwnerWithNullExpiration()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.Owner,al.CheckUserAccess(1,1));
        }

        [TestMethod]
        public void CheckUserAccessIsOwnerWithExpiration()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.Owner, al.CheckUserAccess(1, 2));
        }

        [TestMethod]
        public void CheckUserAccessIsDeniedBecauseExpired()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.NoAccess, al.CheckUserAccess(1, 3));
        }

        [TestMethod]
        public void CheckUserAccessIsOwnerWithNullExpiratio()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.Owner, al.CheckUserAccess(1, 1));
        }



        private IStorageBridge _dbStorage;

        #region Setup
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

        [TestMethod]
        public void GetExpirationDate_BuyerButExpired()
        {
            try
            {
                var authLogic = new AuthLogic(_dbStorage);
                authLogic.GetExpirationDate(3, 2);
                Assert.Fail("Expected InstanceNotFoundException");
            }
            catch (InstanceNotFoundException e)
            {
                Assert.AreEqual("The access right has expired", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected InstanceNotFoundException but got " + e.GetType());
            }
            
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
