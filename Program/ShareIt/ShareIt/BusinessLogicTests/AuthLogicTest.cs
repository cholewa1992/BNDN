using System;
using System.Collections.Generic;
using BusinessLayerTest;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogicTests
{
    [TestClass]
    public class AuthLogicTest: BaseTest
    {


        private IAuthInternalLogic al;


        [TestInitialize]
        public void Initiate()
        {
             var testData = new HashSet<IEntityDto>
             {
                 new Client()
                 {
                     Id = 10,
                     Name = "testClient",
                     Token = "testToken"
                 },
                 new Client()
                 {
                     Name = "testClient2",
                     Token = "testToken2"
                 },
                 new UserAcc()
                 {
                     Id = 1,
                     Username = "username",
                     Password = "password"
                 },
                 new UserAcc()
                 {
                     Username = "username2",
                     Password = "password2"
                 }
             };

            var bridgeStub = new StorageBridgeStub(testData);

            al = new AuthLogic(bridgeStub);
        }

        [TestMethod]
        public void ClientTokenIsWithSystem()
        {
            Assert.AreEqual(1, al.CheckClientToken("testToken"));
        }

        [TestMethod]
        public void testtest()
        {
            Assert.AreEqual(1,1);
        }
    }
}
