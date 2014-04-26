using System;
using System.Linq;
using System.ServiceModel;
using BusinessLogicLayer;
using BusinessLogicLayer.Exceptions;
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
    }
}
