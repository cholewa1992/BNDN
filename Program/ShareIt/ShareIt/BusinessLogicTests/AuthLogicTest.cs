
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
                     Token = "testToken",
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
                     Password = "password",
                     
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
                 },
                 new ClientAdmin()
                 {
                     UserId = 1,
                     Client = new Client()
                     {
                         Token = "testToken2"
                     }
                 }
             };

            var bridgeStub = new StorageBridgeStub(testData);

            al = new AuthLogic(bridgeStub);

            Setup();
        }

        [TestMethod]
        public void ClientTokenExistence_ValidToken_CorrespondingClientId()
        {
            Assert.AreEqual(1, al.CheckClientToken("testToken"));
        }

        [TestMethod]
        public void ClientTokenExistence_InvalidToken_DefaultInteger()
        {
            Assert.AreEqual(-1, al.CheckClientToken("noToken"));
        }

        [TestMethod]
        public void UserAccessExistence_ValidIdsNullExpiration_Granted()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.Owner,al.CheckUserAccess(1,1));
        }

        [TestMethod]
        public void UserAccessExistence_ValidIdsFutureExpiration_Granted()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.Owner, al.CheckUserAccess(1, 2));
        }

        [TestMethod]
        public void UserAccessExistence_ValidIdsOverdueExpiration_NoAccess()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.NoAccess, al.CheckUserAccess(1, 3));
        }

        [TestMethod]
        public void UserAccessExistence_InvalidIds_NoAccess()
        {
            Assert.AreEqual(BusinessLogicLayer.AccessRightType.NoAccess, al.CheckUserAccess(500, 500));
        }

        [TestMethod]
        public void AdminOnClient_Exists_Granted()
        {
            Assert.AreEqual(true,al.IsUserAdminOnClient(1, "testToken2"));
        }

        [TestMethod]
        public void AdminOnClient_NonExistant_Denied()
        {
            Assert.AreEqual(false, al.IsUserAdminOnClient(2, "testToken3"));
        }

        [TestMethod]
        public void AdminOnClient_OnlyIdExists_Denied()
        {
            Assert.AreEqual(false, al.IsUserAdminOnClient(1, "testToken3"));
        }

        [TestMethod]
        public void AdminOnClient_OnlyTokenExists_Denied()
        {
            Assert.AreEqual(false, al.IsUserAdminOnClient(2, "testToken2"));
        }


        [TestMethod]
        public void UserExistence_PassedValidUsernamePassword_True()
        {
            Assert.AreEqual(true, al.CheckUserExists(new UserDTO(){Username = "username", Password = "password"}));
        }

        [TestMethod]
        public void UserExistence_InvalidPassword_False()
        {
            Assert.AreEqual(false, al.CheckUserExists(new UserDTO() { Username = "username", Password = "InvalidPassword" }));
        }

        [TestMethod]
        public void UserExistence_InvalidUsername_False()
        {
            Assert.AreEqual(false, al.CheckUserExists(new UserDTO() { Username = "InvalidUsername", Password = "password" }));
        }

        [TestMethod]
        public void UserExistence_EmptyUsername_Exception()
        {
            Throws<ArgumentException>(() => al.CheckUserExists(new UserDTO() { Username = "", Password = "password" }),
                "Precondition failed: !string.IsNullOrEmpty(user.Username)");
        }

        [TestMethod]
        public void UserExistence_EmptyPassword_Exception()
        {
            Throws<ArgumentException>(() => al.CheckUserExists(new UserDTO() { Username = "username", Password = "" }),
                "Precondition failed: !string.IsNullOrEmpty(user.Password)");
        }


        [TestMethod]
        public void ClientExistence_ValidNameAndToken_True()
        {
            Assert.AreEqual(true, al.CheckClientExists(new ClientDTO(){Name = "testClient", Token = "testToken"}));
        }

        [TestMethod]
        public void ClientExistence_InvalidName_False()
        {
            Assert.AreEqual(false, al.CheckClientExists(new ClientDTO() { Name = "InvalidTestClient", Token = "testToken" }));
        }

        [TestMethod]
        public void ClientExistence_InvalidToken_False()
        {
            Assert.AreEqual(false, al.CheckClientExists(new ClientDTO() { Name = "testClient", Token = "InvalidTestToken" }));
        }

        [TestMethod]
        public void ClientExistence_EmptyName_Exception()
        {
            Throws<ArgumentException>(() => al.CheckClientExists(new ClientDTO() { Name = "", Token = "testToken" }),
                "Precondition failed: !string.IsNullOrEmpty(client.Name)");
        }

        [TestMethod]
        public void ClientExistence_EmptyToken_Exception()
        {
            Throws<ArgumentException>(() => al.CheckClientExists(new ClientDTO() { Name = "testClient", Token = "" }),
                "Precondition failed: !string.IsNullOrEmpty(client.Token)");
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
