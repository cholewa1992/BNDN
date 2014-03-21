using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareIt;

namespace ServiceTests
{
    [TestClass]
    public class AuthServiceTest
    {
        readonly AuthService _authService = new AuthService(BusinessLogicFacade.GetTestFactory());


        [TestMethod]
        public void TestValidateUser()
        {
            var r = _authService.ValidateUser(new User(), "token");
            Assert.AreEqual<bool>(true, r);
        }

        [TestMethod]
        public void TestCheckClientPassword()
        {
            var r = _authService.CheckClientExists(new Client());
            Assert.AreEqual<bool>(true, r);
        }
    }
}
