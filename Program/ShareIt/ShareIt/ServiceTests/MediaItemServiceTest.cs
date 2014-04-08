using System;
using System.Linq;
using System.ServiceModel;
using BusinessLogicLayer;
using BusinessLogicLayer.FaultDataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareIt;

namespace ServiceTests
{
    [TestClass]
    public class MediaItemServiceTest
    {
        private MediaItemService _mediaItemService = new MediaItemService(BusinessLogicFacade.GetTestFactory());

        [TestMethod]
        public void TestGetMediaItemValidId_FileExtension()
        {
            var mediaItem = _mediaItemService.GetMediaItemInformation(1, null, "token");
            Assert.AreEqual(".pdf", mediaItem.FileExtension);
        }

        [TestMethod]
        public void TestGetMediaItemValidId_InformationCount()
        {
            var mediaItem = _mediaItemService.GetMediaItemInformation(1, null, "token");
            Assert.AreEqual(5, mediaItem.Information.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<MediaItemNotFound>))]
        public void TestGetMediaItemInvalidId()
        {
            _mediaItemService.GetMediaItemInformation(9999, null, "token");
            
            /*catch (FaultException<ArgumentFault> e)
            {
                Assert.AreEqual("No media item with id 999 exists in the database", e.Message);
            }
            Assert.Fail("Expected FaultException<ArgumentFault>");*/
        }


    }
}
