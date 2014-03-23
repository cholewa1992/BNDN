using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Collections.Generic;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogicTests
{
    /// <summary>
    /// Summary description for MediaItemLogicTest
    /// </summary>
    [TestClass]
    public class MediaItemLogicTest
    {
        private readonly MediaItemLogic _mediaItemLogic = new MediaItemLogic(null);//(IStorageBridge);
        
        [TestInitialize]
        public void Initialize()
        {
            var info1 = new MediaItemInformation()
            {
                Id = 1,
                Data = "Dansk",
                Type = InformationType.Language
            };

            var info2 = new MediaItemInformation()
            {
                Id = 1,
                Data = "20",
                Type = InformationType.Price
            };

            var infoList = new List<MediaItemInformation>();
            infoList.Add(info1);
            infoList.Add(info2);

            MediaItem _mediaItem = new MediaItem()
            {
                FileExtension = ".avi",
                Id = 1,
                Information = infoList,
                Type = MediaItemType.Movie
            };
        }

        [TestMethod]
        public void GetMediaItemInformation_InvalidMediaItemId()
        {
            const int mediaItemId = 2;

            try
            {
                _mediaItemLogic.GetMediaItemInformation(mediaItemId, "token");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("No media item with id " + mediaItemId + " exists in the database", ae.Message);
            }

            Assert.Fail("Expected ArgumentException");
            
        }

        [TestMethod]
        public void GetMediaItemInformation_MediaItemFetched()
        {
            var mediaItemId = 1;

            MediaItem m = _mediaItemLogic.GetMediaItemInformation(mediaItemId, "token");

            Assert.AreEqual(m.Id, mediaItemId);

        }

        [TestMethod]
        public void GetMediaItemInformation_CorrectInformationDataFetched()
        {
            var mediaItemId = 1;

            MediaItem m = _mediaItemLogic.GetMediaItemInformation(mediaItemId, "token");

            var list = new List<String>();

            foreach (var info in m.Information)
            {
                list.Add(info.Data);
            }

            Assert.AreEqual(list[0], "Dansk");
            Assert.AreEqual(list[1], "20");
        }

        [TestMethod]
        public void GetMediaItemInformation_CorrectInformationTypesFetched()
        {
            var mediaItemId = 1;

            MediaItem m = _mediaItemLogic.GetMediaItemInformation(mediaItemId, "token");

            var list = new List<InformationType>();

            foreach (var info in m.Information)
            {
                list.Add(info.Type);
            }

            Assert.AreEqual(list[0], InformationType.Language);
            Assert.AreEqual(list[1], InformationType.Price);
        }
    }
}
