using System;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthTest
{
    [TestClass]
    public class AuthLogicTest: BaseTest
    {

        AuthLogic al = new AuthLogic();

        [TestMethod]
        public void TestNoUserNamePrecondition()
        {
            Throws<ArgumentException>(() => 
                al.CheckUserExists(new User() { Password = "testpass" }),
                @"Precondition failed: !string.IsNullOrEmpty(user.Username)"
                );
        }

        [TestMethod]
        public void TestNoUserPasswordPrecondition()
        {
            Throws<ArgumentException>(() =>
                al.CheckUserExists(new User() { Username = "John"}),
                @"Precondition failed: !string.IsNullOrEmpty(user.Password)"
                );
        }
    }
}
