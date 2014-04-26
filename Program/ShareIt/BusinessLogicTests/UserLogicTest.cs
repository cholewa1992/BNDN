using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Moq;

namespace BusinessLogicTests
{
    [TestClass]
    public class UserLogicTest
    {
        private IAuthInternalLogic _authLogic;
        private IStorageBridge _dbStorage;
        private UserLogic _userLogic;
        private UserDTO _testUser;

        [TestInitialize]
        public void Initialize()
        {
            SetupAuthMock();
            SetupDbStorageMock();
            _userLogic = new UserLogic(_dbStorage, _authLogic);

            _testUser = new UserDTO();
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
            authMoq.Setup(foo => foo.CheckUserAccess(1, 1)).Returns(BusinessLogicLayer.AccessRightType.NoAccess);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 2)).Returns(BusinessLogicLayer.AccessRightType.Owner);
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i == 2), It.Is<string>(s => s == "testClient")))
                .Returns(true);
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i != 2), It.Is<string>(s => s != "testClient")))
                .Returns(false);
            _authLogic = authMoq.Object;
        }

        private void SetupDbStorageMock()
        {
            var userAcc = new HashSet<UserAcc>();
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(foo => foo.Add(It.IsAny<Entity>())).Verifiable();
            dbMoq.Setup(foo => foo.Update(It.IsAny<Entity>())).Verifiable();

            dbMoq.Setup(foo => foo.Add(It.IsAny<UserAcc>())).Callback<UserAcc>(entity =>
            {
                entity.Id = 1;
                userAcc.Add(entity);
            });

            dbMoq.Setup(foo => foo.Get<UserAcc>()).Returns(userAcc.AsQueryable);
            dbMoq.Setup(foo => foo.Get<UserAcc>(It.IsAny<int>()))
                .Returns<int>(id => userAcc.Single(acc => acc.Id == id));

            dbMoq.Setup(foo => foo.Delete<Entity>(It.IsAny<int>())).Callback(
                new Action<int>(e => userAcc.RemoveWhere(a => a.Id == 1))).Verifiable();

            _dbStorage = dbMoq.Object;
        }

        #region CreateAccount tests

        [TestMethod]
        public void CreateAccount_UsernameTooLong()
        {
            _testUser.Username = "MyNameIsJohnAndIAm44YearsOld";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "testClient");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must consist of between 1 and 20 characters", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void CreateAccount_UsernameIsEmptyString()
        {
            _testUser.Username = "";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "testClient");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must consist of between 1 and 20 characters", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void CreateAccount_UsernameContainsWierdCharacters()
        {
            _testUser.Username = "John-Dollar$";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "testClient");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must only consist of alphanumerical characters (a-zA-Z0-9)", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void CreateAccount_UsernameContainsWhitespace()
        {
            _testUser.Username = "John 44";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "testClient");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must only consist of alphanumerical characters (a-zA-Z0-9)", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void CreateAccount_PasswordTooLong()
        {
            _testUser.Username = "John44";
            _testUser.Password = "IF***ingLoveScience-ItsSooooAmazingHorseSteamBoatWilly";

            try
            {
                _userLogic.CreateAccount(_testUser, "testClient");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Password must consist of between 1 and 50 characters", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void CreateAccount_PasswordIsEmptyString()
        {
            _testUser.Username = "John44";
            _testUser.Password = "";

            try
            {
                _userLogic.CreateAccount(_testUser, "testClient");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Password must consist of between 1 and 50 characters", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void CreateAccount_PasswordContainsWhitespace()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Pass word";

            try
            {
                _userLogic.CreateAccount(_testUser, "testClient");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Password must not contain any whitespace characters", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }


        }

        [TestMethod]
        public void CreateAccount_UsernameAlreadyTakenCaseSensitive()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Hello1234";

            _userLogic.CreateAccount(_testUser, "testClient");

            var anotherUser = new UserDTO()
            {
                Id = 2,
                Username = "John44",
                Password = "YouShallNotPass"
            };

            try
            {
                _userLogic.CreateAccount(anotherUser, "testClient");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Username already in use", e.Message);
            }
        }

        [TestMethod]
        public void CreateAccount_UsernameAlreadyTakenCaseInsensitive()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Hello1234";

            _userLogic.CreateAccount(_testUser, "testClient");

            var anotherUser = new UserDTO()
            {
                Id = 2,
                Username = "john44",
                Password = "YouShallNotPass"
            };

            try
            {
                _userLogic.CreateAccount(anotherUser, "testClient");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Username already in use", e.Message);
            }
        }

        [TestMethod]
        public void CreateAccount_UserIsCreated()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Password";

            var shouldBeTrue = _userLogic.CreateAccount(_testUser, "testClient");
            Assert.AreEqual(shouldBeTrue, true);

            var dbResult =
                _dbStorage.Get<UserAcc>()
                    .Single(t => t.Username == _testUser.Username && t.Password == _testUser.Password);

            Assert.AreEqual(_testUser.Username, dbResult.Username);
            Assert.AreEqual(_testUser.Password, dbResult.Password);

        }

        #endregion

        #region GetAccountInformation tests

        [TestMethod]
        public void GetAccountInformation_targetUserNotFound()
        {
            _testUser.Id = 1;
            _testUser.Username = "John44";
            _testUser.Password = "Password";

            _userLogic.CreateAccount(_testUser, "testClient");

            _testUser.Id = 12;

            try
            {
                _userLogic.GetAccountInformation(_testUser, _testUser.Id, "testClient");
            }
            catch (Exception e)
            {
                Assert.AreEqual("The requested user could not be found", e.Message);
            }
        }

        [TestMethod]
        public void GetAccountInformation_ValidUserInformationReturned()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Password";
            _testUser.Id = 1;

            var requestingUser = new UserDTO
            {
                Id = 5,
                Username = "Jacob12345",
                Password = "helloWorld"
            };

            Assert.IsTrue(_userLogic.CreateAccount(_testUser, "testClient"));
            var u = _userLogic.GetAccountInformation(requestingUser, _testUser.Id, "testClient");

            Assert.AreEqual("John44", u.Username);
            Assert.AreEqual("Password", u.Password);
        }

        #endregion

        #region UpdateAccountInformation

        [TestMethod]
        public void UpdateAccountInformation_UserNotFoundInDB()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Password";

            _userLogic.CreateAccount(_testUser, "testClient");

            _testUser.Id = 12;

            try
            {
                _userLogic.UpdateAccountInformation(_testUser, _testUser, "testClient");
            }
            catch (Exception e)
            {
                Assert.AreEqual("User to be updated was not found in the database", e.Message);
            }
        }

        [TestMethod]
        public void UpdateAccountInformation_UserUpdated()
        {
            _testUser.Id = 1;
            _testUser.Username = "John44";
            _testUser.Password = "Password";
            _testUser.Information = new List<UserInformationDTO>();

            _userLogic.CreateAccount(_testUser, "testClient");

            _testUser.Password = "NytPassword";

            var shouldBeTrue = _userLogic.UpdateAccountInformation(_testUser, _testUser, "testClient");

            Assert.AreEqual(shouldBeTrue, true);

            var dbResult = _dbStorage.Get<UserAcc>(_testUser.Id);

            Assert.AreEqual(dbResult.Username, _testUser.Username);
            Assert.AreEqual(dbResult.Password, _testUser.Password);

        }

        #endregion

        #region GetAllUsers tests

        [TestMethod]
        public void Test_GetAllUsers_ReturnsEmptyListWhenNoUsers()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(true);
            var dbMock = new Mock<IStorageBridge>();
            dbMock.Setup(x => x.Get<UserAcc>()).Returns(new List<UserAcc>().AsQueryable());

            var target = new UserLogic(dbMock.Object, authMock.Object);

            var output = target.GetAllUsers(new UserDTO() {Password = "bla", Username = "blabla"}, "anystring");

            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void Test_GetAllUsers_ReturnsUserNameAndIdOfAllUsers()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(true);
            var dbMock = new Mock<IStorageBridge>();
            var returnList = new List<UserAcc>();
            for (int i = 0; i < 5; i++)
            {
                returnList.Add(new UserAcc()
                {
                    Username = "user" + i,
                    Id = i
                });
            }
            dbMock.Setup(x => x.Get<UserAcc>()).Returns(returnList.AsQueryable());

            var target = new UserLogic(dbMock.Object, authMock.Object);

            var output =
                target.GetAllUsers(new UserDTO() {Password = "bla", Username = "blabla"}, "anystring").ToArray();

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("user" + i, output[i].Username);
                Assert.AreEqual(i, output[i].Id);
            }
        }

        [TestMethod]
        public void Test_GetAllUsers_DoesntMapPassword()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(true);
            var dbMock = new Mock<IStorageBridge>();
            var returnList = new List<UserAcc>();
            for (int i = 0; i < 5; i++)
            {
                returnList.Add(new UserAcc()
                {
                    Username = "user" + i,
                    Id = i
                });
            }
            dbMock.Setup(x => x.Get<UserAcc>()).Returns(returnList.AsQueryable());

            var target = new UserLogic(dbMock.Object, authMock.Object);

            var output =
                target.GetAllUsers(new UserDTO() {Password = "bla", Username = "blabla"}, "anystring").ToArray();

            for (int i = 0; i < 5; i++)
            {
                Assert.IsNull(output[i].Password);
            }
        }

        [ExpectedException(typeof (InvalidClientException))]
        [TestMethod]
        public void Test_GetAllUsers_ThrowsExceptionWhenClientTokenInvalid()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(-1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(true);
            var dbMock = new Mock<IStorageBridge>();
            var returnList = new List<UserAcc>();
            dbMock.Setup(x => x.Get<UserAcc>()).Returns(returnList.AsQueryable());

            var target = new UserLogic(dbMock.Object, authMock.Object);

            target.GetAllUsers(new UserDTO() {Password = "bla", Username = "blabla"}, "anystring");
        }

        [ExpectedException(typeof (InvalidUserException))]
        [TestMethod]
        public void Test_GetAllUsers_ThrowsExceptionWhenUserNotValid()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(-1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(true);
            var dbMock = new Mock<IStorageBridge>();
            var returnList = new List<UserAcc>();
            dbMock.Setup(x => x.Get<UserAcc>()).Returns(returnList.AsQueryable());

            var target = new UserLogic(dbMock.Object, authMock.Object);

            target.GetAllUsers(new UserDTO() {Password = "bla", Username = "blabla"}, "anystring");
        }

        [ExpectedException(typeof (UnauthorizedUserException))]
        [TestMethod]
        public void Test_GetAllUsers_ThrowsExceptionWhenUserNotAdmin()
        {
            var authMock = new Mock<IAuthInternalLogic>();
            authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(false);
            var dbMock = new Mock<IStorageBridge>();
            var returnList = new List<UserAcc>();
            dbMock.Setup(x => x.Get<UserAcc>()).Returns(returnList.AsQueryable());

            var target = new UserLogic(dbMock.Object, authMock.Object);

            target.GetAllUsers(new UserDTO() {Password = "bla", Username = "blabla"}, "anystring");
        }

        #endregion

        // CreateAccount_UserIsNull
        // CreateAccount_ClientTokenIsNull
        // CreateAccount_ClientTokenNotValid

        // GetAccountInformation_RequstingUserIsNull
        // GetAccountInformation_TargetUserIsNull
        // GetAccountInformation_ClientTokenIsNotValid

        // UpdateAccountInformation_RequstingUserIsNull
        // UpdateAccountInformation_TargetUserIsNull
        // UpdateAccountInformation_ClientTokenIsNotValid


        #region DeleteUser

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void DeleteUser_UserIsNull_ArgumentNullException()
        {
            _userLogic.DeleteUser(null, 1, "testClient");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void DeleteUser_TokenIsNull_ArgumentNullException()
        {
            _userLogic.DeleteUser(_testUser, 1, null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void DeleteUser_InvalidUserId_ArgumentException()
        {
            _userLogic.DeleteUser(_testUser, -3, "testClient");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void DeleteUser_UsernameNull_ArgumentException()
        {
            _testUser.Id = 1;
            _testUser.Username = null;
            _testUser.Password = "testPassword";
            _userLogic.DeleteUser(_testUser, 1, "testClient");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void DeleteUser_PasswordNull_ArgumentException()
        {
            _testUser.Id = 1;
            _testUser.Username = "testUserName";
            _testUser.Password = null;
            _userLogic.DeleteUser(_testUser, 1, "testClient");
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidClientException))]
        public void DeleteUser_InvalidClientToken_UnauthorizedClient()
        {
            _testUser.Id = 1;
            _testUser.Username = "testUserName";
            _testUser.Password = "testPassword";
            _userLogic.DeleteUser(_testUser, 1, "invalidToken");
        }

        [TestMethod]
        [ExpectedException(typeof (UnauthorizedUserException))]
        public void DeleteUser_InvalidUserCredentials_UnauthorizedUser()
        {
            _testUser.Id = 1;
            _testUser.Username = "invalidName";
            _testUser.Password = "testPassword";
            _userLogic.DeleteUser(_testUser, 1, "testClient");
        }

        [TestMethod]
        [ExpectedException(typeof (UnauthorizedUserException))]
        public void DeleteUser_UserDeletingOtherUser_UnauthorizedUser()
        {
            var userToBeDeleted = new UserDTO {Id = 2, Username = "DeleteMe", Password = "del1234"};
            Assert.IsTrue(_userLogic.CreateAccount(userToBeDeleted, "testClient"));
            _testUser.Id = 1;
            _testUser.Username = "testUserName";
            _testUser.Password = "testPassword";
            _userLogic.DeleteUser(_testUser, 2, "testClient");
        }

        [TestMethod]
        public void DeleteUser_UserDeletingSelf_Success()
        {
            _testUser.Id = 1;
            _testUser.Username = "testUserName";
            _testUser.Password = "testPassword";
            Assert.IsTrue(_userLogic.DeleteUser(_testUser, 1, "testClient"));
        }

        [TestMethod]
        public void DeleteUser_AdminDeletingUser_Success()
        {
            //var authMock = new Mock<IAuthInternalLogic>();
            //authMock.Setup(x => x.CheckClientToken(It.IsAny<string>())).Returns(1);
            //authMock.Setup(x => x.CheckUserExists(It.IsAny<UserDTO>())).Returns(1);
            //authMock.Setup(x => x.IsUserAdminOnClient(It.IsAny<UserDTO>(), It.IsAny<string>())).Returns(true);
            //var target = new UserLogic(_dbStorage, authMock.Object);
            //_testUser.Id = 2;
            //_testUser.Username = "testUserName";
            //_testUser.Password = "testPassword";
            //target.CreateAccount(_testUser, "testClient");
            //var before = target.GetAllUsers(_testUser, "testClient").Count;
            //Assert.IsTrue(target.DeleteUser(_testUser, 1, "testClient"));
            //Assert.AreEqual(before - 1, target.GetAllUsers(_testUser, "testClient").Count);

            _testUser.Id = 2;
            _testUser.Username = "testUserName";
            _testUser.Password = "testPassword";
            _userLogic.CreateAccount(_testUser, "testClient");
            Assert.IsTrue(_userLogic.DeleteUser(_testUser, 1, "testClient"));
        }


    #endregion
    }
}
