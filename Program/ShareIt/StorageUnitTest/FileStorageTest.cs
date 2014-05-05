using System;
using System.IO;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StorageUnitTest
{
    [TestClass]
    public class FileStorageTest
    {
        [TestMethod]
        public void TestSaveMediaStoresFileAtLocation()
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("This is a sample string");
            var ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            
            var target = new FileStorage();
            var result = target.SaveMedia(ms, 1, 1, ".txt");
            ms.Close();

            //Assertion
            Assert.IsTrue(File.Exists(result));

            File.Delete(result);

        }
    }
}
