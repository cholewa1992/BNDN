using System;
using System.Linq;
using System.Runtime.InteropServices;
using ArtShare.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareItServices.UserService;

namespace ClientUnitTest
{
    [TestClass]
    public class AccountLogicTest
    {
        private AccountLogic _accountLogic = new AccountLogic();

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void DeleteAccount_UsernameNull_ArgumentNullException()
        {
            _accountLogic.GetAllUsers(null, "password");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void DeleteAccount_PasswordNull_ArgumentNullException()
        {
            _accountLogic.GetAllUsers("username", null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void GetAllUsers_UsernameNull_ArgumentNullException()
        {
            _accountLogic.GetAllUsers(null, "password");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void GetAllUsers_PasswordNull_ArgumentNullException()
        {
            _accountLogic.GetAllUsers("username", null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ExtractAccountInformation_UserNull_ArgumentNullException()
        {
            _accountLogic.ExtractAccountInformation(null);
        }

        [TestMethod]
        public void ExtractAccountInformation_UserWithInformation_AccountModel()
        {
            var user = new UserDTO
            {
                Id = 1,
                Username = "SuperUser",
                Password = "pw1234",
                Information = new UserInformationDTO[]
                {
                    new UserInformationDTO {Id = 1, Type = UserInformationTypeDTO.Firstname, Data = "Anders"},
                    new UserInformationDTO {Id = 2, Type = UserInformationTypeDTO.Lastname, Data = "Andersen"},
                    new UserInformationDTO {Id = 3, Type = UserInformationTypeDTO.Email, Data = "aa@itu.dk"},
                    new UserInformationDTO {Id = 3, Type = UserInformationTypeDTO.Location, Data = "Copenhagen"}
                }
            };
            var accountModel = _accountLogic.ExtractAccountInformation(user);

            Assert.AreEqual(user.Id, accountModel.Id);
            Assert.AreEqual(user.Username, accountModel.Username);
            Assert.AreEqual(user.Password, accountModel.Password);
            Assert.AreEqual(user.Information[0].Data, accountModel.Firstname);
            Assert.AreEqual(user.Information[1].Data, accountModel.Lastname);
            Assert.AreEqual(user.Information[2].Data, accountModel.Email);
            Assert.AreEqual(user.Information[3].Data, accountModel.Location);
        }

        [TestMethod]
        public void ExtractAccountInformation_UserWithoutInformation_AccountModel()
        {
            var user = new UserDTO
            {
                Id = 1,
                Username = "SuperUser",
                Password = "pw1234"
            };
            var accountModel = _accountLogic.ExtractAccountInformation(user);
            Assert.AreEqual(user.Id, accountModel.Id);
            Assert.AreEqual(user.Username, accountModel.Username);
            Assert.AreEqual(user.Password, accountModel.Password);
            Assert.AreEqual("", accountModel.Firstname);
            Assert.AreEqual("", accountModel.Lastname);
            Assert.AreEqual("", accountModel.Email);
            Assert.AreEqual("", accountModel.Location);
        }

    }
}
