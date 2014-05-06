using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AccessRightType = DataAccessLayer.AccessRightType;

namespace BusinessLogicTests
{
    /// <author>
    /// Asbjørn Steffensen (afjs@itu.dk)
    /// </author>
    [TestClass]
    public class AccessRightLogicTest
    {


        #region Setup

        private IStorageBridge _bridgeStub;
        private IAccessRightInternalLogic accessRightLogic;
        private IAuthInternalLogic _authLogic;

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
                     Username = "user1",
                     Password = "pass1"
                     
                 },
                 new UserAcc()
                 {
                     Id = 2,
                     Username = "user2",
                     Password = "pass2"
                 },
                 new UserAcc()
                 {
                     Id = 3,
                     Username = "user3",
                     Password = "pass3"
                 },
                 new ClientAdmin()
                 {
                     ClientId = 1,
                     Id = 1,
                     UserId = 2
                 },
                 new Entity()
                 {
                     Id = 1,
                     ClientId = 1
                 },
                 new Entity()
                 {
                     Id = 2,
                     ClientId = 1
                 },
                 new Entity()
                 {
                     Id = 3,
                     ClientId = 1
                 },
                 new Entity()
                 {
                     Id = 4,
                     ClientId = 1
                 },
                 new AccessRight()
                 {
                     Id = 2,
                     EntityId = 2,
                     UserId = 2,
                     Expiration = null,
                     AccessRightTypeId = 1
                 },
                 new AccessRight()
                 {
                     Id = 3,
                     EntityId = 3,
                     UserId = 3,
                     Expiration = null,
                     AccessRightTypeId = 1
                 },
                 new AccessRight()
                 {
                     Id = 4,
                     EntityId = 4,
                     UserId = 3,
                     Expiration = null,
                     AccessRightTypeId = 2
                 },
                 new AccessRightType()
                 {
                     Id = 1,
                     Name = "Owner"
                 },
                 new AccessRightType()
                 {
                     Id = 2,
                     Name = "Buyer"
                 },
             };

            _bridgeStub = new StorageBridgeStub(testData);

            SetupAuthMock();

            accessRightLogic = new AccessRightLogic(_authLogic, _bridgeStub);
        }

        private void SetupAuthMock()
        {
            var authMoq = new MockRepository(MockBehavior.Default).Create<IAuthInternalLogic>();
            //setup checkClientToken.
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s == "testClient"))).Returns(1);
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s != "testClient"))).Returns(-1);
            //setup checkUserExists.
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password == "pass1" && u.Username == "user1"))).Returns(1);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password == "pass2" && u.Username == "user2"))).Returns(2);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password == "pass3" && u.Username == "user3"))).Returns(3);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password != "pass1" && u.Username == "user1"))).Returns(-1);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password != "pass2" && u.Username == "user2"))).Returns(-1);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password != "pass3" && u.Username == "user3"))).Returns(-1);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Username != "user1" && u.Username != "user2" && u.Username != "user3"))).Returns(-1);
            //setup checkUserAccess
            authMoq.Setup(foo => foo.CheckUserAccess(1, 1)).Returns(BusinessLogicLayer.AccessRightType.NoAccess);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 2)).Returns(BusinessLogicLayer.AccessRightType.Owner);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 3)).Returns(BusinessLogicLayer.AccessRightType.Buyer);
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i == 2), It.Is<string>(s => s == "testClient"))).Returns(true);
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i != 2), It.Is<string>(s => s != "testClient"))).Returns(false);
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(1, 1)).Returns((DateTime?)null);
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(1, 2)).Returns((DateTime?)null);
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(2, 2)).Returns(new DateTime(2015, 01, 01, 00, 00, 00));
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(1, 3)).Throws<InstanceNotFoundException>();
            _authLogic = authMoq.Object;
        }
        #endregion

        #region Purchase

        [TestMethod]
        public void Purchase_CorrectUserId()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };
            
            accessRightLogic.Purchase(user, 1, null, "testClient");

            var ar1 =_bridgeStub.Get<AccessRight>().Single(x => x.EntityId == 1);

            Assert.AreEqual(user.Id, ar1.UserId);
        }

        [TestMethod]
        public void Purchase_CorrectEntityId()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.Purchase(user, 1, null, "testClient");

            var ar1 = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 1);

            Assert.AreEqual(1, ar1.EntityId);
        }

        [TestMethod]
        public void Purchase_CorrectAccessRightType()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.Purchase(user, 1, null, "testClient");

            var ar1 = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 1);

            Assert.AreEqual(1, ar1.AccessRightTypeId);
        }

        [ExpectedException(typeof(MediaItemNotFoundException))]
        [TestMethod]
        public void Purchase_MediaItemNotFound()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.Purchase(user, 202020, null, "testClient");
        }

        [ExpectedException(typeof(InvalidUserException))]
        [TestMethod]
        public void Purchase_InvalidUser()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "invalid",
                Username = "invalid"
            };

            accessRightLogic.Purchase(user, 1, null, "testClient");
        }

        [ExpectedException(typeof(InvalidClientException))]
        [TestMethod]
        public void Purchase_InvalidClient()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.Purchase(user, 1, null, "invalid");
        }

        #endregion

        #region MakeAdmin

        [TestMethod]
        public void MakeAdmin_CorrectUserId()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            accessRightLogic.MakeAdmin(user, 1, "testClient");

            var admin = _bridgeStub.Get<ClientAdmin>().Single(x => x.UserId == 1);

            Assert.AreEqual(1, admin.UserId);
        }

        [TestMethod]
        public void MakeAdmin_CorrectClientId()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            accessRightLogic.MakeAdmin(user, 1, "testClient");

            var admin = _bridgeStub.Get<ClientAdmin>().Single(x => x.UserId == 1);

            Assert.AreEqual(1, admin.ClientId);
        }

        [ExpectedException(typeof(UnauthorizedUserException))]
        [TestMethod]
        public void MakeAdmin_InvokingUserNotAdmin()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.MakeAdmin(user, 1, "testClient");
        }

        [ExpectedException(typeof(UserNotFoundException))]
        [TestMethod]
        public void MakeAdmin_UserNotFound()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            accessRightLogic.MakeAdmin(user, 202020, "testClient");
        }

        [ExpectedException(typeof(InvalidUserException))]
        [TestMethod]
        public void MakeAdmin_InvalidUser()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "invalid",
                Username = "invalid"
            };

            accessRightLogic.MakeAdmin(user, 202020, "testClient");
        }

        [ExpectedException(typeof(InvalidClientException))]
        [TestMethod]
        public void MakeAdmin_InvalidClient()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.MakeAdmin(user, 202020, "invalid");
        }

        #endregion

        #region DeleteAccessRight

        [TestMethod]
        public void DeleteAccessRight_AccessRightDeleted()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            accessRightLogic.DeleteAccessRight(user, 2, "testClient");

            var accessRights = _bridgeStub.Get<AccessRight>().Where(x => x.Id == 2);

            Assert.AreEqual(0, accessRights.Count());
        }

        [ExpectedException(typeof(UnauthorizedUserException))]
        [TestMethod]
        public void DeleteAccessRight_InvokingUserNotAdmin()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.DeleteAccessRight(user, 1, "testClient");
        }

        [ExpectedException(typeof(AccessRightNotFoundException))]
        [TestMethod]
        public void DeleteAccessRight_AccessRightNotFound()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            accessRightLogic.DeleteAccessRight(user, 202020, "testClient");
        }

        [ExpectedException(typeof(InvalidUserException))]
        [TestMethod]
        public void DeleteAccessRight_InvalidUser()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "invalid",
                Username = "invalid"
            };

            accessRightLogic.DeleteAccessRight(user, 202020, "testClient");
        }

        [ExpectedException(typeof(InvalidClientException))]
        [TestMethod]
        public void DeleteAccessRight_InvalidClient()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.DeleteAccessRight(user, 202020, "invalid");
        }

        #endregion

        #region GetPurchaseHistory

        [TestMethod]
        public void GetPurchaseHistory_CorrectAccessRightId_Admin()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var accessRightDTOs = accessRightLogic.GetPurchaseHistory(user, 3, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 3 && x.AccessRightTypeId == 1);

            Assert.AreEqual(accessRightDTOs.First().Id, accessRight.Id);
        }

        [TestMethod]
        public void GetPurchaseHistory_CorrectAccessRightId_Owner()
        {
            var user = new UserDTO()
            {
                Id = 3,
                Password = "pass3",
                Username = "user3"
            };

            var accessRightDTOs = accessRightLogic.GetPurchaseHistory(user, 3, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 3 && x.AccessRightTypeId == 1);

            Assert.AreEqual(accessRightDTOs.First().Id, accessRight.Id);
        }

        [TestMethod]
        public void GetPurchaseHistory_CorrectMediaItemId()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var accessRightDTOs = accessRightLogic.GetPurchaseHistory(user, 3, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 3 && x.AccessRightTypeId == 1);

            Assert.AreEqual(accessRightDTOs.First().MediaItemId, accessRight.EntityId);
        }

        [TestMethod]
        public void GetPurchaseHistory_CorrectUserId()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var accessRightDTOs = accessRightLogic.GetPurchaseHistory(user, 3, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 3 && x.AccessRightTypeId == 1);

            Assert.AreEqual(accessRightDTOs.First().UserId, accessRight.UserId);
        }

        [ExpectedException(typeof(UnauthorizedUserException))]
        [TestMethod]
        public void GetPurchaseHistory_InvokingUserNotAdminOrOwner()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.GetPurchaseHistory(user, 3, "testClient");
        }

        [ExpectedException(typeof(InvalidUserException))]
        [TestMethod]
        public void GetPurchaseHistory_InvalidUser()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "invalid",
                Username = "invalid"
            };

            accessRightLogic.GetPurchaseHistory(user, 202020, "testClient");
        }

        [ExpectedException(typeof(InvalidClientException))]
        [TestMethod]
        public void GetPurchaseHistory_InvalidClient()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.GetPurchaseHistory(user, 202020, "invalid");
        }

        #endregion

        #region GetUploadHistory

        [TestMethod]
        public void GetUploadHistory_CorrectAccessRightId()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var accessRightDTOs = accessRightLogic.GetUploadHistory(user, 3, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 3 && x.AccessRightTypeId == 2);

            Assert.AreEqual(accessRightDTOs.First().Id, accessRight.Id);
        }

        [TestMethod]
        public void GetUploadHistory_CorrectMediaItemId()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var accessRightDTOs = accessRightLogic.GetUploadHistory(user, 3, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 3 && x.AccessRightTypeId == 2);

            Assert.AreEqual(accessRightDTOs.First().MediaItemId, accessRight.EntityId);
        }

        [TestMethod]
        public void GetUploadHistory_CorrectUserId()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var accessRightDTOs = accessRightLogic.GetUploadHistory(user, 3, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.UserId == 3 && x.AccessRightTypeId == 2);

            Assert.AreEqual(accessRightDTOs.First().UserId, accessRight.UserId);
        }
        
        [ExpectedException(typeof(InvalidClientException))]
        [TestMethod]
        public void GetUploadHistory_InvalidClient()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            accessRightLogic.GetUploadHistory(user, 202020, "invalid");
        }

        #endregion

        #region EditExpiration

        [TestMethod]
        public void EditExpiration_AccessRightUpdated_Admin()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var newAR = new AccessRightDTO()
            {
                Expiration = new DateTime(01, 01, 01),
                Id = 3,
                MediaItemId = 3,
                UserId = 3
            };

            accessRightLogic.EditExpiration(user, newAR, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.Id == 3);

            Assert.AreEqual(newAR.Expiration, accessRight.Expiration);
        }

        [TestMethod]
        public void EditExpiration_AccessRightUpdated_Owner()
        {
            var user = new UserDTO()
            {
                Id = 3,
                Password = "pass3",
                Username = "user3"
            };

            var newAR = new AccessRightDTO()
            {
                Expiration = new DateTime(01, 01, 01),
                Id = 3,
                MediaItemId = 3,
                UserId = 3
            };

            accessRightLogic.EditExpiration(user, newAR, "testClient");

            var accessRight = _bridgeStub.Get<AccessRight>().Single(x => x.Id == 3);

            Assert.AreEqual(newAR.Expiration, accessRight.Expiration);
        }

        [ExpectedException(typeof(UnauthorizedUserException))]
        [TestMethod]
        public void EditExpiration_InvokingUserNotAdmin()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            var newAR = new AccessRightDTO()
            {
                Expiration = new DateTime(01, 01, 01),
                Id = 3,
                MediaItemId = 3,
                UserId = 3
            };

            accessRightLogic.EditExpiration(user, newAR, "testClient");
        }

        [ExpectedException(typeof(AccessRightNotFoundException))]
        [TestMethod]
        public void EditExpiration_AccessRightNotFound()
        {
            var user = new UserDTO()
            {
                Id = 2,
                Password = "pass2",
                Username = "user2"
            };

            var newAR = new AccessRightDTO()
            {
                Expiration = new DateTime(01, 01, 01),
                Id = 5,
                MediaItemId = 5,
                UserId = 5
            };

            accessRightLogic.EditExpiration(user, newAR, "testClient");
        }

        [ExpectedException(typeof(InvalidUserException))]
        [TestMethod]
        public void EditExpiration_InvalidUser()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "invalid",
                Username = "invalid"
            };

            var newAR = new AccessRightDTO()
            {
                Expiration = new DateTime(01, 01, 01),
                Id = 3,
                MediaItemId = 3,
                UserId = 3
            };

            accessRightLogic.EditExpiration(user, newAR, "testClient");
        }

        [ExpectedException(typeof(InvalidClientException))]
        [TestMethod]
        public void EditExpiration_InvalidClient()
        {
            var user = new UserDTO()
            {
                Id = 1,
                Password = "pass1",
                Username = "user1"
            };

            var newAR = new AccessRightDTO()
            {
                Expiration = new DateTime(01, 01, 01),
                Id = 3,
                MediaItemId = 3,
                UserId = 3
            };

            accessRightLogic.EditExpiration(user, newAR, "invalid");
        }

        #endregion

        #region CanDownload tests

        [TestMethod]
        public void Test_CanDownload_ReturnsTrueIfUserIsAdmin()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var target = new AccessRightLogic(authMock.Object, _bridgeStub);

            var output = target.CanDownload(new UserDTO(), 3, "client");

            Assert.IsTrue(output);
        }

        [TestMethod]
        public void Test_CanDownload_ReturnsTrueIfUserOwner()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(false);
            authMock.Setup(x => x.CheckUserAccess(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(BusinessLogicLayer.AccessRightType.Owner);

            var target = new AccessRightLogic(authMock.Object, _bridgeStub);

            var output = target.CanDownload(new UserDTO(), 3, "client");

            Assert.IsTrue(output);
        }

        [TestMethod]
        public void Test_CanDownload_ReturnsTrueIfUserIsBuyer()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(false);
            authMock.Setup(x => x.CheckUserAccess(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(BusinessLogicLayer.AccessRightType.Buyer);

            var target = new AccessRightLogic(authMock.Object, _bridgeStub);

            var output = target.CanDownload(new UserDTO(), 3, "client");

            Assert.IsTrue(output);
        }

        [TestMethod]
        public void Test_CanDownload_ReturnsFalseIfUserHasNoAccess()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(false);
            authMock.Setup(x => x.CheckUserAccess(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(BusinessLogicLayer.AccessRightType.NoAccess);

            var target = new AccessRightLogic(authMock.Object, _bridgeStub);

            var output = target.CanDownload(new UserDTO(), 3, "client");

            Assert.IsFalse(output);
        }
        [ExpectedException(typeof(InvalidClientException))]
        [TestMethod]
        public void Test_CanDownload_ThrowsExceptionIfClientTokenInvalid()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(-1);

            var target = new AccessRightLogic(authMock.Object, _bridgeStub);
            target.CanDownload(new UserDTO(), 3, "client");

        }
        [ExpectedException(typeof(InvalidUserException))]
        [TestMethod]
        public void Test_CanDownload_ThrowsExceptionIfUserInvalid()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(-1);

            var target = new AccessRightLogic(authMock.Object, _bridgeStub);

            target.CanDownload(new UserDTO(), 3, "client");
        }
        #endregion
    }
}
