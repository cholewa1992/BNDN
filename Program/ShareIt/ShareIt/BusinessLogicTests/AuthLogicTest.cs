using System;
using BusinessLayerTest;
using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogicTests
{
    [TestClass]
    public class AuthLogicTest: BaseTest
    {

        IAuthInternalLogic al = new AuthLogic(new StorageBridge(new EfStorageConnection<RentIt08Entities>()));

        [TestMethod]
        public void TestMethod1()
        {
            
        }

        
    }
}
