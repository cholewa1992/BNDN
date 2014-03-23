using System;
using System.IO;
using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BusinessLogicTests
{
    [TestClass]
    public class DataTransferLogicTest
    {
        [TestMethod]
        public void TestSaveMediaMapsMediaItemCorrectly()
        {

            //setup
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(foo => foo.Add(It.IsAny<Entity>())).Verifiable();
            dbMoq.Setup(foo => foo.Update(It.IsAny<Entity>())).Verifiable();
            var fileMoq = new Mock<IFileStorage>();
            fileMoq.Setup(foo => foo.SaveFile(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns("TestFilePath");

        }
    }
}
