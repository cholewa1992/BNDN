using System;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLayerTest
{
    [TestClass]
    public class AuthLogicTest: BaseTest
    {

        private AuthLogic al = new AuthLogic();

        [TestMethod]
        public void testCheckUserAccess_NullArgument()
        {

            Throws<ArgumentNullException>(() =>
                al.CheckUserExists(new User() { Password = "testpass" }),
                @"Precondition failed: !string.IsNullOrEmpty(user.Username)"
                );

        }


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
                al.CheckUserExists(new User() { Username = "John" }),
                @"Precondition failed: !string.IsNullOrEmpty(user.Password)"
                );
        }
    }
}
