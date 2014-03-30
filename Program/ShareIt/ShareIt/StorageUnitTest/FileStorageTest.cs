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
            //setup
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("Test");
                stream.Position = 0;
                //execution
                var target = new FileStorage();
                var result = target.SaveMedia(stream, 1, 1, ".txt");
                //Assertion
                Assert.IsTrue(File.Exists(result));

                File.Delete(result);
            }

        }
    }
}
