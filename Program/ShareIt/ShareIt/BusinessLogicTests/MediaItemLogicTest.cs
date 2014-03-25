﻿using System;
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
        private readonly MediaItemLogic _mediaItemLogic = new MediaItemLogic(null, null);//(IStorageBridge);
        
        [TestInitialize]
        public void Initialize()
        {
            var info1 = new MediaItemInformationDTO()
            {
                Id = 1,
                Data = "Dansk",
                Type = InformationTypeDTO.Language
            };

            var info2 = new MediaItemInformationDTO()
            {
                Id = 1,
                Data = "20",
                Type = InformationTypeDTO.Price
            };

            var infoList = new List<MediaItemInformationDTO>();
            infoList.Add(info1);
            infoList.Add(info2);

            MediaItemDTO _mediaItem = new MediaItemDTO()
            {
                FileExtension = ".avi",
                Id = 1,
                Information = infoList,
                Type = MediaItemTypeDTO.Movie
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

            MediaItemDTO m = _mediaItemLogic.GetMediaItemInformation(mediaItemId, "token");

            Assert.AreEqual(m.Id, mediaItemId);

        }

        [TestMethod]
        public void GetMediaItemInformation_CorrectInformationDataFetched()
        {
            var mediaItemId = 1;

            MediaItemDTO m = _mediaItemLogic.GetMediaItemInformation(mediaItemId, "token");

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

            MediaItemDTO m = _mediaItemLogic.GetMediaItemInformation(mediaItemId, "token");

            var list = new List<InformationTypeDTO>();

            foreach (var info in m.Information)
            {
                list.Add(info.Type);
            }

            Assert.AreEqual(list[0], InformationTypeDTO.Language);
            Assert.AreEqual(list[1], InformationTypeDTO.Price);
        }

        [TestMethod]
        private void FindMediaItemRange_FromLessThanTo_ItemCount()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
            var bookList = dictionary[MediaItemTypeDTO.Book];
            Assert.AreEqual(to - (from - 1), bookList.MediaItemList.Count);  //Assuming that the number of books exceed the range
        }

        [TestMethod]
        private void FindMediaItemRange_FromGreaterThanTo()
        {
            const int from = 3;
            const int to = 1;
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
            var bookList = dictionary[MediaItemTypeDTO.Book];
            Assert.AreEqual(from - (to - 1), bookList.MediaItemList.Count); //Assuming that the number of books exceed the range
        }

        [TestMethod]
        private void FindMediaItemRange_ToIsNull()
        {
            try
            {
                const int from = 1;
                const int to = 0;
                _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
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
                _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
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
                _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
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
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
            const int numberOfMediaItemTypesWithOneMillionItems = 0; //Assuming that there is not 1000000 items of a specific type
            int numberOfKeyValuePairs = dictionary.Count;
            Assert.AreEqual(numberOfMediaItemTypesWithOneMillionItems, numberOfKeyValuePairs);
        }

        [TestMethod]
        private void FindMediaItemRange_ToExceedsNumberOfElements_ItemCount()
        {
            const int from = 1;
            const int to = 100;
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
            const int numberOfBooks = 10; // Assuming we have exactly 10 books
            var bookList = dictionary[MediaItemTypeDTO.Book];
            Assert.AreEqual(numberOfBooks, bookList.MediaItemList.Count); 
        }

        [TestMethod]
        private void FindMediaItemRange_RangeTooBig()
        {
            try
            {
                const int from = 1;
                const int to = 101;
                _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
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
            _mediaItemLogic.FindMediaItemRange(@from, to, null, null, null);
        }

        [ExpectedException(typeof(Exception))] //TODO Update exception type when CheckClientToken is done
        [TestMethod]
        private void FindMediaItemRange_ClientTokenInvalid()
        {
            const int from = 1;
            const int to = 10;
            _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "invalidToken");
        }

        [TestMethod]
        private void FindMediaItemRange_MediaItemTypeAndSearchKeyAreNull()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, null, null, "token");
            const int numberOfMediaItemTypes = 3; //Books, music, movies
            int numberOfKeyValuePairs = dictionary.Count;
            Assert.AreEqual(numberOfMediaItemTypes, numberOfKeyValuePairs); //Assuming that there is at least one media item per type
        }

        [TestMethod]
        private void FindMediaItemRange_ValidMediaItemTypeSearchKeyIsNull()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, MediaItemTypeDTO.Movie, null, "token");
            const int numberOfMovies = 10; //Assuming there are exactly 10 books
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(numberOfMovies, movieList.MediaItemList.Count);
        }

        [TestMethod]
        private void FindMediaItemRange_ValidMediaItemTypeValidSearchKey()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, MediaItemTypeDTO.Movie, "love", "token");
            const int numberOfMoviesThatMatchesSearchKey = 2; //Assuming there are exactly 2 books matching "love"
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(numberOfMoviesThatMatchesSearchKey, movieList.MediaItemList.Count);
        }

        [TestMethod]
        private void FindMediaItemRange_SearchKeyIsWhiteSpace()
        {
            const int from = 1;
            const int to = 3;
            var dictionary = _mediaItemLogic.FindMediaItemRange(@from, to, MediaItemTypeDTO.Movie, " ", "token");
            const int numberOfMoviesThatMatchesSearchKey = 8; //Assuming there are exactly 8 books matching " "
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(numberOfMoviesThatMatchesSearchKey, movieList.MediaItemList.Count);
        }
    }
}
