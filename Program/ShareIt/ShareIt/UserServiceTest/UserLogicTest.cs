using System;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareIt;

namespace UserServiceTest
{
    [TestClass]
    public class UserLogicTest
    {

        readonly UserLogic _userLogic = new UserLogic(IStorageBridge);
        private User _testUser;

        [TestInitialize]
        public void Initialize()
        {
            _testUser = new User {Id = 1};
        }
        
        [TestMethod]
        public void CreateAccount_UsernameTooLong()
        {
            _testUser.Username = "MyNameIsJohnAndIAm44YearsOld";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must consist of between 1 and 20 characters", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void CreateAccount_UsernameIsEmptyString()
        {
            _testUser.Username = "";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must consist of between 1 and 20 characters", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void CreateAccount_UsernameContainsWierdCharacters()
        {
            _testUser.Username = "John-Dollar$";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must only consist of alphanumerical characters (a-zA-Z0-9)", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void CreateAccount_UsernameContainsWhitespace()
        {
            _testUser.Username = "John 44";
            _testUser.Password = "hej1234";

            try
            {
                _userLogic.CreateAccount(_testUser, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Username must only consist of alphanumerical characters (a-zA-Z0-9)", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void CreateAccount_PasswordTooLong()
        {
            _testUser.Username = "John44";
            _testUser.Password = "IF***ingLoveScience-ItsSooooAmazingHorseSteamBoatWilly";

            try
            {
                _userLogic.CreateAccount(_testUser, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Password must consist of between 1 and 50 characters", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void CreateAccount_PasswordIsEmptyString()
        {
            _testUser.Username = "John44";
            _testUser.Password = "";

            try
            {
                _userLogic.CreateAccount(_testUser, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Password must consist of between 1 and 50 characters", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void CreateAccount_PasswordContainsWhitespace()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Pass word";

            try
            {
                _userLogic.CreateAccount(_testUser, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Password must not contain any whitespace characters", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
        }

        [TestMethod]
        public void CreateAccount_UsernameAlreadyTakenCaseSensitive()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Hello1234";

            _userLogic.CreateAccount(_testUser, "token");

            var anotherUser = new User()
            {
                Id = 2,
                Username = "John44",
                Password = "YouShallNotPass"
            };

            try
            {
                _userLogic.CreateAccount(anotherUser, "token");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Username already in use", e.Message);
            }

            Assert.Fail("Expected Exception");
            
        }

        [TestMethod]
        public void CreateAccount_UsernameAlreadyTakenCaseInsensitive()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Hello1234";

            _userLogic.CreateAccount(_testUser, "token");

            var anotherUser = new User()
            {
                Id = 2,
                Username = "john44",
                Password = "YouShallNotPass"
            };

            try
            {
                _userLogic.CreateAccount(anotherUser, "token");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Username already in use", e.Message);
            }

            Assert.Fail("Expected Exception");
        }

        [TestMethod]
        public void CreateAccount_UserIsCreated()
        {
            _testUser.Username = "John44";
            _testUser.Password = "Password";

            var shouldBeTrue = _userLogic.CreateAccount(_testUser, "token");

            Assert.AreEqual(shouldBeTrue, true);

            // Alternative test if the user is in the testdb

            
        }
    }
}
