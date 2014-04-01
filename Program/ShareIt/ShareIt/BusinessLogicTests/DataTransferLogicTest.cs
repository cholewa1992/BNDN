using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AccessRightType = BusinessLogicLayer.AccessRightType;

namespace BusinessLogicTests
{
    [TestClass]
    public class DataTransferLogicTest
    {
        private IAuthInternalLogic _authLogic;
        private IStorageBridge _dbStorage;
        private IFileStorage _fileStorage;
        private DataTransferLogic _target;
#region Init
        [TestInitialize]
        public void Init()
        {
            SetupAuthMock();
            SetupDbStorageMock();
            SetupFileStorageMock();
            _target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
        }
        private void SetupAuthMock()
        {
            var authMoq = new MockRepository(MockBehavior.Default).Create<IAuthInternalLogic>();
            //setup checkClientToken.
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s == "testClient"))).Returns(1);
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s != "testClient"))).Returns(-1);
            //setup checkUserExists.
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<UserDTO>(u => u.Password == "testPassword" && u.Username == "testUserName")))
                .Returns(1);
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<UserDTO>(u => u.Password != "testPassword" && u.Username == "testUserName")))
                .Returns(-1);
            //setup checkUserAccess
            authMoq.Setup(foo => foo.CheckUserAccess(1, 2)).Returns(BusinessLogicLayer.AccessRightType.NoAccess);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 1)).Returns(BusinessLogicLayer.AccessRightType.Owner);
            _authLogic = authMoq.Object;
        }

        private void SetupDbStorageMock()
        {
            _dbStorage = new StorageBridgeStub(new HashSet<IEntityDto>());
        }

        private void SetupFileStorageMock()
        {
            var fileMoq = new Mock<IFileStorage>();
            fileMoq.Setup(foo => foo.SaveMedia(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns("TestFilePath").Verifiable();
            fileMoq.Setup(foo => foo.ReadFile(It.IsAny<string>())).Returns(new MemoryStream()).Verifiable();
            fileMoq.Setup(x => x.SaveThumbnail(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<string>())).Returns("testURL").Verifiable();
            _fileStorage = fileMoq.Object;
        }
#endregion

        #region SaveMedia tests
        [TestMethod]
        [ExpectedException(typeof(FaultException<UnauthorizedClient>))]
        public void TestSaveMediaThrowsExceptionWhenClientTokenNotAuthorized()
        {
            var logic = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            logic.SaveMedia("Notvalid", new UserDTO(){Password = "something", Username = "something"}, new MediaItemDTO(){FileExtension = "something"}, new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof (FaultException<UnauthorizedUser>))]
        public void TestSaveMediaThrowsExceptionWhenUserCredentialsNotAuthorized()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("testClient", new UserDTO {Username = "Notvalid", Password = "NotValid"}, new MediaItemDTO(){FileExtension = "something"},
                new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenClientTokenIsNull()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia(null, new UserDTO(), new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenUserIsNull()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", null, new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenMediaItemIsNull()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO(), null, new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenStreamIsNull()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO(), new MediaItemDTO(), null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestSaveMediaThrowsArgumentExceptionWhenUsernameIsNull()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO{Username = null, Password = "NotValid"}, new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestSaveMediaThrowsArgumentExceptionWhenUserNameIsEmpty()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO { Username = "", Password = "NotValid" }, new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestSaveMediaThrowsArgumentExceptionWhenUserNameIsWhitespace()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO { Username = "     ", Password = "NotValid" }, new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSaveMediaThrowsArgumentExceptionWhenUserPasswordIsNull()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO { Username = "NotValid", Password = null }, new MediaItemDTO(), new MemoryStream());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSaveMediaThrowsArgumentExceptionWhenUserPasswordIsEmpty()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO { Username = "NotValid", Password = "" }, new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSaveMediaThrowsArgumentExceptionWhenUserPasswordIsWhitespace()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("token", new UserDTO { Username = "NotValid", Password = "     " }, new MediaItemDTO(), new MemoryStream());
        }

        #endregion
        #region SaveThumbnailTests

        [TestMethod]
        public void TestSaveThumbnailReturnsUrlAsFromStorage()
        {
            var entity = new Entity {Id = 1};
            _dbStorage.Add(entity);
            var result = _target.SaveThumbnail("testClient", new UserDTO() {Username = "testUserName", Password = "testPassword"}, 1,
                ".jpg", new MemoryStream());
            Assert.AreEqual("testURL", result);
            SetupDbStorageMock();
        }

        [TestMethod]
        public void TesetSaveTumbnailStoresNewEntityInfoWithCorrectEntityIdAndType()
        {
            var entity = new Entity { Id = 1 };
            _dbStorage.Add(entity);

            _target.SaveThumbnail("testClient", new UserDTO() {Username = "testUserName", Password = "testPassword"}, 1,
                ".jpg", new MemoryStream());
            var info = _dbStorage.Get<EntityInfo>().First();
            Assert.AreEqual(info.EntityId, 1);
            Assert.AreEqual(info.EntityInfoTypeId, (int) InformationTypeDTO.Thumbnail);
            SetupDbStorageMock();
        }

        [TestMethod]
        public void TestSaveThumbnailStoresOnlyOneEntityInfoInStorage()
        {
            var entity = new Entity { Id = 1 };
            _dbStorage.Add(entity);

            _target.SaveThumbnail("testClient", new UserDTO() { Username = "testUserName", Password = "testPassword" }, 1,
                ".jpg", new MemoryStream());

            Assert.AreEqual(_dbStorage.Get<EntityInfo>().Count(), 1);
            SetupDbStorageMock();
        }
        #endregion
    }
}
