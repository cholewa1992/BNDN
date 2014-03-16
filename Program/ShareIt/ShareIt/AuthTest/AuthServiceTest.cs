using System;
using System.Runtime.InteropServices;
using BusinessLogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareItServices;

namespace AuthTest
{
    [TestClass]
    public class AuthServiceTest
    {

        AuthService authService = new AuthService(BusinessLogicFacade.GetTestFactory());

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
