using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            dbMoq.Setup(foo => foo.Get<UserAcc>(It.IsAny<int>())).Returns<int>(id => userAcc.Single(acc => acc.Id == id));

            _dbStorage = dbMoq.Object;
        }

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
                _dbStorage.Get<UserAcc>().Single(t => t.Username == _testUser.Username && t.Password == _testUser.Password);
 
            Assert.AreEqual(_testUser.Username, dbResult.Username);
            Assert.AreEqual(_testUser.Password, dbResult.Password);

        }

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

        // CreateAccount_UserIsNull
        // CreateAccount_ClientTokenIsNull
        // CreateAccount_ClientTokenNotValid

        // GetAccountInformation_RequstingUserIsNull
        // GetAccountInformation_TargetUserIsNull
        // GetAccountInformation_ClientTokenIsNotValid

        // UpdateAccountInformation_RequstingUserIsNull
        // UpdateAccountInformation_TargetUserIsNull
        // UpdateAccountInformation_ClientTokenIsNotValid

    }
}
