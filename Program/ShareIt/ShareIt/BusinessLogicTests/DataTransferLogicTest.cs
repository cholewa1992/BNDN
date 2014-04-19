using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.ServiceModel;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
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
        private Mock<IAuthInternalLogic> _authLogic;
        private IStorageBridge _dbStorage;
        private Mock<IFileStorage> _fileStorage;
#region Init
        [TestInitialize]
        public void Init()
        {
            SetupAuthMock();
            SetupDbStorageMock();
            SetupFileStorageMock();
        }
        private void SetupAuthMock()
        {
            var authMoq = new MockRepository(MockBehavior.Default).Create<IAuthInternalLogic>();
            //setup checkClientToken.
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s == "testClient"))).Returns(1);
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s != "testClient"))).Returns(-1);
            _authLogic = authMoq;
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
            fileMoq.Setup(x => x.SaveThumbnail(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<string>())).Returns("testURL").Verifiable();
            _fileStorage = fileMoq;
        }
#endregion

        #region SaveMedia tests
        [TestMethod]
        [ExpectedException(typeof(InvalidClientException))]
        public void TestSaveMediaThrowsExceptionWhenClientTokenNotAuthorized()
        {
            var logic = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            logic.SaveMedia("Notvalid", new UserDTO() { Password = "something", Username = "something" }, new MediaItemDTO() { FileExtension = "something" }, new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidUserException))]
        public void TestSaveMediaThrowsExceptionWhenUserCredentialsNotAuthorized()
        {
            SetupCheckUserExists(-1);
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveMedia("testClient", new UserDTO { Username = "Notvalid", Password = "NotValid" }, new MediaItemDTO() { FileExtension = "something" },
                new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenClientTokenIsNull()
        {
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveMedia(null, new UserDTO(), new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenUserIsNull()
        {
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveMedia("token", null, new MediaItemDTO(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenMediaItemIsNull()
        {
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveMedia("token", new UserDTO(), null, new MemoryStream());
        }

        [TestMethod]
        public void TestSaveMediaCallsAddAndUpdateOnStorage()
        {
            int userId = 123;
            SetupCheckUserExists(userId);
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(x => x.Add(It.IsAny<Entity>())).Verifiable();
            dbMoq.Setup(x => x.Update(It.IsAny<Entity>())).Verifiable();

            var target = new DataTransferLogic(_fileStorage.Object, dbMoq.Object, _authLogic.Object);
            target.SaveMedia("testClient", new UserDTO(), new MediaItemDTO(){FileExtension = ".txt"}, new MemoryStream());
            //Assert.
            dbMoq.Verify(x => x.Add(It.IsAny<Entity>()), Times.Once);
            dbMoq.Verify(x => x.Update(It.IsAny<Entity>()), Times.Once);
        }

        [TestMethod]
        public void TestSaveMediaCallsDeleteOnDb_IfIoExceptionFromFileStorage()
        {
            int userId = 12523;
            SetupCheckUserExists(userId);
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(x => x.Add(It.IsAny<Entity>())).Verifiable();
            dbMoq.Setup(x => x.Delete(It.IsAny<Entity>())).Verifiable();

            _fileStorage.Setup(x => x.SaveMedia(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Throws<IOException>();

            var target = new DataTransferLogic(_fileStorage.Object, dbMoq.Object, _authLogic.Object);
            target.SaveMedia("testClient", new UserDTO(), new MediaItemDTO() {FileExtension = "bla"}, new MemoryStream());

            dbMoq.Verify(x => x.Add(It.IsAny<Entity>()), Times.Once);
            dbMoq.Verify(x => x.Delete(It.IsAny<Entity>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSaveMediaThrowsNullExceptionWhenStreamIsNull()
        {
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveMedia("token", new UserDTO(), new MediaItemDTO(), null);
        }

        #endregion
        #region SaveThumbnailTests

        [TestMethod]
        public void TestSaveThumbnailReturnsUrlAsFromStorage()
        {
            int userId = 456;
            int mediaId = 5607;
            SetupCheckUserExists(userId);
            SetupUserIsOwner(userId, mediaId);
            var entity = new Entity { Id = mediaId };
            _dbStorage.Add(entity);
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            var result = target.SaveThumbnail("testClient", new UserDTO() { Username = "testUserName", Password = "testPassword" }, mediaId,
                ".jpg", new MemoryStream());
            Assert.AreEqual("testURL", result);
            SetupDbStorageMock();
        }

        [TestMethod]
        public void TestSaveTumbnailStoresNewEntityInfoWithCorrectEntityIdAndType()
        {
            int userId = 1;
            int mediaId = 12;
            SetupCheckUserExists(userId);
            SetupUserIsOwner(userId, mediaId);
            var entity = new Entity { Id = mediaId };
            _dbStorage.Add(entity);
            
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveThumbnail("testClient", new UserDTO() { Username = "testUserName", Password = "testPassword" }, mediaId,
                ".jpg", new MemoryStream());
            var info = _dbStorage.Get<EntityInfo>().First();
            Assert.AreEqual(info.EntityId, mediaId);
            Assert.AreEqual(info.EntityInfoTypeId, (int)InformationTypeDTO.Thumbnail);
            SetupDbStorageMock();
        }

        [TestMethod]
        public void TestSaveThumbnailStoresOnlyOneEntityInfoInStorage()
        {
            int userId = 9876;
            int mediaId = 123566;
            SetupCheckUserExists(userId);
            SetupUserIsAdmin(userId);
            var entity = new Entity { Id = mediaId };
            _dbStorage.Add(entity);
            
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveThumbnail("testClient", new UserDTO() { Username = "testUserName", Password = "testPassword" }, mediaId,
                ".jpg", new MemoryStream());

            Assert.AreEqual(_dbStorage.Get<EntityInfo>().Count(), 1);
            SetupDbStorageMock();
        }

        [TestMethod]
        [ExpectedException(typeof (UnauthorizedUserException))]
        public void TestSaveThumbnailThrowsExceptionWhenUserNotOwnerOrAdmin()
        {
            int userId = 12456;
            int mediaId = 356;
            SetupCheckUserExists(userId);
            SetupUserIsBuyer(userId, mediaId);
            var entity = new Entity { Id = mediaId };
            _dbStorage.Add(entity);

            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.SaveThumbnail("testClient", new UserDTO() { Username = "testUserName", Password = "testPassword" }, mediaId,
                ".jpg", new MemoryStream());
        }

        #endregion
        #region GetMediaStream tests

        [TestMethod]
        public void TestGetMediaStreamAllowsAdmin()
        {
            //setup autlogic
            int userId = 1;
            SetupCheckUserExists(userId);
            SetupUserIsAdmin(userId);
            SetupUserNoAccess();
            //Setup filestorage
            var returnStream = new MemoryStream();
            SetupReadFile(returnStream);
            //setup dbstorage
            int mediaId = 1;
            var returnEntity = new Entity() {Id = mediaId, FilePath = "myPath.txt"};
            _dbStorage.Add(returnEntity);
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            //execute
            string fileExtension;
            var result = target.GetMediaStream("testClient", new UserDTO {Username = "name", Password = "password"}, mediaId,
                out fileExtension);
            //assert
            Assert.AreEqual(returnStream, result);
            Assert.AreEqual(".txt", fileExtension);
            //cleanup
            SetupDbStorageMock();
        }

        [TestMethod]
        public void TestGetMediaStreamAllowsOwner()
        {
            int userId = 1;
            int mediaId = 1;
            SetupCheckUserExists(userId);
            SetupUserIsOwner(userId, mediaId);
            
            //Setup filestorage
            var returnStream = new MemoryStream();
            SetupReadFile(returnStream);
            var returnEntity = new Entity() { Id = mediaId, FilePath = "myPath.txt" };
            _dbStorage.Add(returnEntity);
            //execute
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            string fileExtension;
            var result = target.GetMediaStream("testClient", new UserDTO { Username = "name", Password = "password" }, mediaId,
                out fileExtension);

            //assert
            Assert.AreEqual(returnStream, result);
            Assert.AreEqual(".txt", fileExtension);
            //cleanup
            SetupDbStorageMock();

        }

        [TestMethod]
        public void TestGetMediaStreamAllowsBuyer()
        {
            int userId = 10;
            int mediaId = 30;
            SetupCheckUserExists(userId);
            SetupUserIsBuyer(userId, mediaId);
            //Setup filestorage
            var returnStream = new MemoryStream();
            SetupReadFile(returnStream);
            var returnEntity = new Entity() { Id = mediaId, FilePath = "myPath.txt" };
            _dbStorage.Add(returnEntity);
            //execute
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            string fileExtension;
            var result = target.GetMediaStream("testClient", new UserDTO { Username = "name", Password = "password" }, mediaId,
                out fileExtension);

            //assert
            Assert.AreEqual(returnStream, result);
            Assert.AreEqual(".txt", fileExtension);
            //cleanup
            SetupDbStorageMock();
        }
        
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedUserException))]
        public void TestGetMediaStreamThrowsExceptionWhenUserHasNoAccess()
        {
            int userId = 999;
            int mediaId = 12345;
            SetupCheckUserExists(999);
            SetupUserNoAccess();

            //execute
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            string fileExtension;
            var result = target.GetMediaStream("testClient", new UserDTO {Username = "name", Password = "password"},
                mediaId,
                out fileExtension);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidClientException))]
        public void TestGetMediaStreamThrowsExceptionWhenUserCredentialsInvalid()
        {
            int userId = -1;
            SetupCheckUserExists(userId);

            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            string fileExtension;
            target.GetMediaStream("testClint", new UserDTO(), 12, out fileExtension);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidClientException))]
        public void TestGetMediaStreamThrowsExceptionWhenClientTokenInvalid()
        {
            string fileExtension;
            var target = new DataTransferLogic(_fileStorage.Object, _dbStorage, _authLogic.Object);
            target.GetMediaStream("notTestClient", new UserDTO(), 1234, out fileExtension);
        }
        #endregion

        #region helper methods
        private void SetupUserIsAdmin(int userId)
        {
            _authLogic.Setup(x => x.IsUserAdminOnClient(It.Is<int>(n => n == userId), It.Is<string>(s => s == "testClient"))).Returns(true);
        }

        private void SetupCheckUserExists(int userIdToReturn)
        {
            _authLogic.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(userIdToReturn);
        }

        private void SetupReadFile(Stream streamToReturn)
        {
            _fileStorage.Setup(x => x.ReadFile(It.Is<string>(s => s == "myPath.txt"))).Returns(streamToReturn);
        }

        private void SetupUserIsBuyer(int userId, int mediaId)
        {
            _authLogic.Setup(x => x.CheckUserAccess(It.Is<int>(n => n == userId), It.Is<int>(m => m == mediaId)))
                .Returns(AccessRightType.Buyer);
        }

        private void SetupUserIsOwner(int userId, int mediaId)
        {
            _authLogic.Setup(x => x.CheckUserAccess(It.Is<int>(n => n == userId), It.Is<int>(m => m == mediaId)))
                .Returns(AccessRightType.Owner);
        }

        private void SetupUserNoAccess()
        {
            _authLogic.Setup(x => x.CheckUserAccess(It.IsAny<int>(), It.IsAny<int>())).Returns(AccessRightType.NoAccess);
        }
        #endregion
    }
}
