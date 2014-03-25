using System;
using System.IO;
using System.Net.Security;
using System.ServiceModel;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BusinessLogicTests
{
    [TestClass]
    public class DataTransferLogicTest
    {
        private IAuthInternalLogic _authLogic;
        private IStorageBridge _dbStorage;
        private IFileStorage _fileStorage;
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
            //setup checkUserExists.
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<UserDTO>(u => u.Password == "testPassword" && u.Username == "testUserName")))
                .Returns(true);
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<UserDTO>(u => u.Password != "testPassword" && u.Username == "testUserName")))
                .Returns(false);
            //setup checkUserAccess
            authMoq.Setup(foo => foo.CheckUserAccess(1, 1)).Returns(BusinessLogicLayer.AccessRightType.NoAccess);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 2)).Returns(BusinessLogicLayer.AccessRightType.Owner);
            _authLogic = authMoq.Object;
        }

        private void SetupDbStorageMock()
        {
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(foo => foo.Add(It.IsAny<Entity>())).Verifiable();
            dbMoq.Setup(foo => foo.Update(It.IsAny<Entity>())).Verifiable();
            _dbStorage = dbMoq.Object;
        }

        private void SetupFileStorageMock()
        {
            var fileMoq = new Mock<IFileStorage>();
            fileMoq.Setup(foo => foo.SaveFile(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns("TestFilePath");
            fileMoq.Setup(foo => foo.ReadFile(It.IsAny<string>())).Returns(new MemoryStream());
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
    }
}
