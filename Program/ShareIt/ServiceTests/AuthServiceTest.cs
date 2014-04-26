using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareIt;

namespace ServiceTests
{
    [TestClass]
    public class AuthServiceTest
    {
        readonly AuthService _authService = new AuthService(BusinessLogicEntryFactory.GetTestFactory());


        [TestMethod]
        public void TestValidateUser()
        {
            var r = _authService.ValidateUser(new UserDTO(), "token");
            Assert.AreEqual<int>(1, r);
        }

        [TestMethod]
        public void TestCheckClientPassword()
        {
            var r = _authService.CheckClientExists(new ClientDTO());
            Assert.AreEqual<bool>(true, r);
        }
    }
}
