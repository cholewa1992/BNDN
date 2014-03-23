using System;
using System.IO;
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
                    foo.CheckUserExists(It.Is<User>(u => u.Password == "testPassword" && u.Username == "testUserName")))
                .Returns(true);
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<User>(u => u.Password != "testPassword" && u.Username == "testUserName")))
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
            logic.SaveMedia("Notvalid", new User(), new MediaItem(), new MemoryStream());
        }

        [TestMethod]
        [ExpectedException(typeof (FaultException<UnauthorizedUser>))]
        public void TestSaveMediaThrowsExceptionWhenUserCredentialsNotCorrect()
        {
            var target = new DataTransferLogic(_fileStorage, _dbStorage, _authLogic);
            target.SaveMedia("testClient", new User {Username = "Notvalid", Password = "NotValid"}, new MediaItem(),
                new MemoryStream());
        }
        #endregion
    }
}
