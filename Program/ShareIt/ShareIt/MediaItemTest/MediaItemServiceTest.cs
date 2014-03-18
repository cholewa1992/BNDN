using System;
using System.Linq;
using BusinessLogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareIt;

namespace MediaItemTest
{
    [TestClass]
    public class MediaItemServiceTest
    {
        private MediaItemService _mediaItemService = new MediaItemService(BusinessLogicFacade.GetTestFactory());

        [TestMethod]
        public void TestGetMediaItemValidId_FileExtension()
        {
            var mediaItem = _mediaItemService.GetMediaItemInformation(1, "token");
            Assert.AreEqual(".pdf", mediaItem.FileExtension);
        }

        [TestMethod]
        public void TestGetMediaItemValidId_InformationCount()
        {
            var mediaItem = _mediaItemService.GetMediaItemInformation(1, "token");
            Assert.AreEqual(2, mediaItem.Information.Count());
        }

        [TestMethod]
        public void TestGetMediaItemInvalidId()
        {
            try
            {
                _mediaItemService.GetMediaItemInformation(999, "token");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("No media item with id 999 exists in the database", e.Message);
            }
            Assert.Fail("Expected ArgumentException");
        }
    }
}
