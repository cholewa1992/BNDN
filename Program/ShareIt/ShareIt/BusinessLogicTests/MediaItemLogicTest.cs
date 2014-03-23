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
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("No media item with id " + mediaItemId + " exists in the database", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
            
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

        [TestMethod]
        private void FindMediaItemRange_FromLessThanTo_ItemCount()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
            var bookList = dictionary[MediaItemType.Book];
            Assert.AreEqual(to - (from - 1), bookList.Count);  //Assuming that the number of books exceed the range
        }

        [TestMethod]
        private void FindMediaItemRange_FromGreaterThanTo()
        {
            const int from = 3;
            const int to = 1;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
            var bookList = dictionary[MediaItemType.Book];
            Assert.AreEqual(from - (to - 1), bookList.Count); //Assuming that the number of books exceed the range
        }

        [TestMethod]
        private void FindMediaItemRange_ToIsNull()
        {
            try
            {
                const int from = 1;
                const int to = 0;
                _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Both \"from\" and \"to\" must be greater than 1", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        private void FindMediaItemRange_FromIsNull()
        {
            try
            {
                const int from = 0;
                const int to = 3;
                _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Both \"from\" and \"to\" must be greater than 1", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        private void FindMediaItemRange_FromAndToAreNull()
        {
            try
            {
                const int from = 0;
                const int to = 0;
                _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Both \"from\" and \"to\" must be greater than 1", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        private void FindMediaItemRange_FromExceedsNumberOfElements_ListCount()
        {
            const int from = 1000000;
            const int to = 1000003;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
            const int numberOfMediaItemTypesWithOneMillionItems = 0; //Assuming that there is not 1000000 items of a specific type
            int numberOfKeyValuePairs = dictionary.Count;
            Assert.AreEqual(numberOfMediaItemTypesWithOneMillionItems, numberOfKeyValuePairs);
        }

        [TestMethod]
        private void FindMediaItemRange_ToExceedsNumberOfElements_ItemCount()
        {
            const int from = 1;
            const int to = 100;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
            const int numberOfBooks = 10; // Assuming we have exactly 10 books
            var bookList = dictionary[MediaItemType.Book];
            Assert.AreEqual(numberOfBooks, bookList.Count); 
        }

        [TestMethod]
        private void FindMediaItemRange_RangeTooBig()
        {
            try
            {
                const int from = 1;
                const int to = 101;
                _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("The range is too big. The cap on the range is 100.", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        private void FindMediaItemRange_ClientTokenNull()
        {
            const int from = 1;
            const int to = 10;
            _mediaItemLogic.FindMediaItemRange(from, to, null, null, null);
        }

        [ExpectedException(typeof(Exception))] //TODO Update exception type when CheckClientToken is done
        [TestMethod]
        private void FindMediaItemRange_ClientTokenInvalid()
        {
            const int from = 1;
            const int to = 10;
            _mediaItemLogic.FindMediaItemRange(from, to, null, null, "invalidToken");
        }

        [TestMethod]
        private void FindMediaItemRange_MediaItemTypeAndSearchKeyAreNull()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, null, null, "token");
            const int numberOfMediaItemTypes = 3; //Books, music, movies
            int numberOfKeyValuePairs = dictionary.Count;
            Assert.AreEqual(numberOfMediaItemTypes, numberOfKeyValuePairs); //Assuming that there is at least one media item per type
        }

        [TestMethod]
        private void FindMediaItemRange_ValidMediaItemTypeSearchKeyIsNull()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, MediaItemType.Movie, null, "token");
            const int numberOfMovies = 10; //Assuming there are exactly 10 books
            var movieList = dictionary[MediaItemType.Movie];
            Assert.AreEqual(numberOfMovies, movieList.Count);
        }

        [TestMethod]
        private void FindMediaItemRange_ValidMediaItemTypeValidSearchKey()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, MediaItemType.Movie, "love", "token");
            const int numberOfMoviesThatMatchesSearchKey = 2; //Assuming there are exactly 2 books matching "love"
            var movieList = dictionary[MediaItemType.Movie];
            Assert.AreEqual(numberOfMoviesThatMatchesSearchKey, movieList.Count);
        }

        [TestMethod]
        private void FindMediaItemRange_SearchKeyIsWhiteSpace()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(from, to, MediaItemType.Movie, " ", "token");
            const int numberOfMoviesThatMatchesSearchKey = 8; //Assuming there are exactly 8 books matching " "
            var movieList = dictionary[MediaItemType.Movie];
            Assert.AreEqual(numberOfMoviesThatMatchesSearchKey, movieList.Count);
        }
    }
}
