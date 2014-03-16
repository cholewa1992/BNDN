using System;
using System.Runtime.InteropServices;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareItServices;

namespace AuthTest
{
    [TestClass]
    public class AuthServiceTest
    {

        AuthService authService = new AuthService(BusinessLogicFacade.GetTestFactory());

        [TestMethod]
        public void TestCheckAccess()
        {
            var r = authService.CheckAccess(new User(), new Client());
            Assert.AreEqual(true, r);
        }
    }
}
