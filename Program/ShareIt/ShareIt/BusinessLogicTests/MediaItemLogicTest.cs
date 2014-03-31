﻿using System;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Authentication;
using System.Text;
using System.Collections.Generic;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BusinessLogicTests
{
    /// <summary>
    /// Summary description for MediaItemLogicTest
    /// </summary>
    [TestClass]
    public class MediaItemLogicTest
    {
        private IAuthInternalLogic _authLogic;
        private IStorageBridge _dbStorage;

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            SetupAuthMock();
            SetupDbStorageMock();
        }

        private void SetupAuthMock()
        {
            var authMoq = new MockRepository(MockBehavior.Default).Create<IAuthInternalLogic>();
            //setup checkClientToken.
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s == "testClient"))).Returns(1);
            authMoq.Setup(foo => foo.CheckClientToken(It.Is<string>(s => s != "testClient"))).Returns(-1);
            //setup checkUserExists.
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<UserDTO>(u => u.Password == "testPassword" && u.Username == "testUserName")))
                .Returns(1);
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<UserDTO>(u => u.Password != "testPassword" && u.Username == "testUserName")))
                .Returns(-1);
            //setup checkUserAccess
            authMoq.Setup(foo => foo.CheckUserAccess(1, 1)).Returns(BusinessLogicLayer.AccessRightType.NoAccess);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 2)).Returns(BusinessLogicLayer.AccessRightType.Owner);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 3)).Returns(BusinessLogicLayer.AccessRightType.Buyer);
            _authLogic = authMoq.Object;
        }

        private void SetupDbStorageMock()
        {
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(foo => foo.Add(It.IsAny<Entity>())).Verifiable();
            dbMoq.Setup(foo => foo.Update(It.IsAny<Entity>())).Verifiable();
            var mediaItems = SetupMediaItems();
            HashSet<EntityInfo> info = new HashSet<EntityInfo>();
            foreach (var item in mediaItems)
            {
                foreach (var entityInfo in item.EntityInfo)
                {
                    info.Add(entityInfo);
                }
            }
            var ratings = SetupRatings();
            dbMoq.Setup(foo => foo.Get<Rating>()).Returns(ratings.AsQueryable);
            dbMoq.Setup(foo => foo.Get<EntityInfo>()).Returns(info.AsQueryable);
            dbMoq.Setup(foo => foo.Get<Entity>()).Returns(mediaItems.AsQueryable);
            _dbStorage = dbMoq.Object;
        }

        private HashSet<Entity> SetupMediaItems()
        {
            //Add some data
            var book1 = new Entity { Id = 1, TypeId = (int)MediaItemTypeDTO.Book, ClientId = 1};
            book1.EntityInfo = new List<EntityInfo>
            {
                new EntityInfo {EntityId = 1, Id = 1, EntityInfoTypeId = 1, Data = "Book1"},
                new EntityInfo { EntityId = 1, Id = 2, EntityInfoTypeId = 2, Data = "Description1" }
            };
            var book2 = new Entity { Id = 2, TypeId = (int)MediaItemTypeDTO.Book, ClientId = 1};
            book2.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 2, Id = 3, EntityInfoTypeId = 1, Data = "Book2"},
                new EntityInfo { EntityId = 2, Id = 4, EntityInfoTypeId = 2, Data = "Description2" } 
            };
            var book3 = new Entity { Id = 3, TypeId = (int)MediaItemTypeDTO.Book, ClientId = 1 };
            book3.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 3, Id = 5, EntityInfoTypeId = 1, Data = "Book3"},
                new EntityInfo { EntityId = 3, Id = 6, EntityInfoTypeId = 2, Data = "Description3" } 
            };
            var book4 = new Entity { Id = 4, TypeId = (int)MediaItemTypeDTO.Book, ClientId = 1 };
            book4.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 4, Id = 7, EntityInfoTypeId = 1, Data = "Book4"},
                new EntityInfo { EntityId = 4, Id = 8, EntityInfoTypeId = 2, Data = "Description4" } 
            };

            var music1 = new Entity { Id = 5, TypeId = (int)MediaItemTypeDTO.Music, ClientId = 1 };
            music1.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 5, Id = 9, EntityInfoTypeId = 1, Data = "Music1"},
                new EntityInfo { EntityId = 5, Id = 10, EntityInfoTypeId = 2, Data = "Description5" },
                new EntityInfo { EntityId = 5, Id = 11, EntityInfoTypeId = 12, Data = "Artist1" }
            };
            var music2 = new Entity { Id = 6, TypeId = (int)MediaItemTypeDTO.Music, ClientId = 1 };
            music2.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 6, Id = 12, EntityInfoTypeId = 1, Data = "Music2"},
                new EntityInfo {EntityId = 6, Id = 13, EntityInfoTypeId = 2, Data = "Description6"},
                new EntityInfo {EntityId = 6, Id = 14, EntityInfoTypeId = 12, Data = "Artist2"}
            };
            var music3 = new Entity { Id = 7, TypeId = (int)MediaItemTypeDTO.Music, ClientId = 1 };
            music3.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 7, Id = 15, EntityInfoTypeId = 1, Data = "Music3"},
                new EntityInfo {EntityId = 7, Id = 16, EntityInfoTypeId = 2, Data = "Description7"},
                new EntityInfo {EntityId = 7, Id = 17, EntityInfoTypeId = 12, Data = "Artist3"}
            };

            var movie1 = new Entity { Id = 8, TypeId = (int)MediaItemTypeDTO.Movie, ClientId = 1 };
            movie1.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 8, Id = 18, EntityInfoTypeId = 1, Data = "Movie1"},
                new EntityInfo {EntityId = 8, Id = 19, EntityInfoTypeId = 2, Data = "Description8"},
                new EntityInfo {EntityId = 8, Id = 20, EntityInfoTypeId = 11, Data = "Director1"}
            };
            var movie2 = new Entity { Id = 9, TypeId = (int)MediaItemTypeDTO.Movie, ClientId = 1 };
            movie2.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 9, Id = 21, EntityInfoTypeId = 1, Data = "Movie2"},
                new EntityInfo {EntityId = 9, Id = 22, EntityInfoTypeId = 2, Data = "Description9"},
                new EntityInfo {EntityId = 9, Id = 23, EntityInfoTypeId = 11, Data = "Director2"}
            };
            var movie3 = new Entity { Id = 10, TypeId = (int)MediaItemTypeDTO.Movie, ClientId = 1 };
            movie3.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 10, Id = 24, EntityInfoTypeId = 1, Data = "Movie3"},
                new EntityInfo {EntityId = 10, Id = 25, EntityInfoTypeId = 2, Data = "Description10"},
                new EntityInfo {EntityId = 10, Id = 26, EntityInfoTypeId = 11, Data = "Director3"}
            };
            var movie4 = new Entity { Id = 11, TypeId = (int)MediaItemTypeDTO.Movie, ClientId = 1 };
            movie4.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 11, Id = 27, EntityInfoTypeId = 1, Data = "Movie4"},
                new EntityInfo {EntityId = 11, Id = 28, EntityInfoTypeId = 2, Data = "Description11"},
                new EntityInfo {EntityId = 11, Id = 29, EntityInfoTypeId = 11, Data = "Director 1"}
            };
            var movie5 = new Entity { Id = 12, TypeId = (int)MediaItemTypeDTO.Movie, ClientId = 1 };
            movie5.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 12, Id = 30, EntityInfoTypeId = 1, Data = "Movie5"},
                new EntityInfo {EntityId = 12, Id = 31, EntityInfoTypeId = 2, Data = "Description12"},
                new EntityInfo {EntityId = 12, Id = 32, EntityInfoTypeId = 11, Data = "Director 2"}
            };

            return new HashSet<Entity>{book1, book2, book3, book4, music1, music2, music3, movie1, movie2, movie3, movie4, movie5};
        }

        private HashSet<Rating> SetupRatings()
        {
            var r1 = new Rating { Id = 1, UserId = 1, EntityId = 1, Value = 10 };
            var r2 = new Rating { Id = 2, UserId = 1, EntityId = 2, Value = 1 };
            var r3 = new Rating { Id = 3, UserId = 2, EntityId = 1, Value = 7 };
            var r4 = new Rating { Id = 4, UserId = 3, EntityId = 1, Value = 2 };
            return new HashSet<Rating> {r1, r2, r3, r4};
        }

            #endregion

        #region GetMediaItemInformation
        //TODO Shouldn't be argumentexception
        [TestMethod]
        public void GetMediaItemInformation_InvalidMediaItemId()
        {
            const int mediaItemId = 202020;
            try
            {
                var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
                mediaItemLogic.GetMediaItemInformation(mediaItemId, null, "testClient");
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
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            
            var mediaItemId = 1;

            MediaItemDTO m = mediaItemLogic.GetMediaItemInformation(mediaItemId, null, "testClient");

            Assert.AreEqual(m.Id, mediaItemId);

        }

        [TestMethod]
        public void GetMediaItemInformation_CorrectInformationDataFetched()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var mediaItemId = 2;

            MediaItemDTO m = mediaItemLogic.GetMediaItemInformation(mediaItemId, null, "testClient");

            var list = new List<String>();

            foreach (var info in m.Information)
            {
                list.Add(info.Data);
            }

            Assert.AreEqual(list[0], "Book2");
            Assert.AreEqual(list[1], "Description2");
        }

        [TestMethod]
        public void GetMediaItemInformation_CorrectInformationTypesFetched()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var mediaItemId = 1;

            MediaItemDTO m = mediaItemLogic.GetMediaItemInformation(mediaItemId, null, "testClient");

            var list = new List<InformationTypeDTO>();

            foreach (var info in m.Information)
            {
                list.Add(info.Type);
            }

            Assert.AreEqual(list[0], InformationTypeDTO.Title);
            Assert.AreEqual(list[1], InformationTypeDTO.Description);
        }
        #endregion

        #region FindMediaItemRange
        [TestMethod]
        public void FindMediaItemRange_FromLessThanTo_ItemCount()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
            var bookList = dictionary[MediaItemTypeDTO.Book];
            Assert.AreEqual(to - (from - 1), bookList.MediaItemList.Count); //TODO Assuming only books? otherwise the first 3 hits wouldn't necessarily be books?
        }

        [TestMethod]
        public void FindMediaItemRange_FromGreaterThanTo()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 3;
            const int to = 1;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
            var bookList = dictionary[MediaItemTypeDTO.Book];
            Assert.AreEqual(from - (to - 1), bookList.MediaItemList.Count); //Assuming that the number of books exceed the range
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void FindMediaItemRange_ToIsZero()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 0;
            mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void FindMediaItemRange_FromIsZero()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 0;
            const int to = 3;
            mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void FindMediaItemRange_FromAndToAreNull() //TODO Redundant when you have already check seperately?
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 0;
            const int to = 0;
            mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
        }

        [TestMethod]
        public void FindMediaItemRange_FromExceedsNumberOfElements_ListCount()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1000000;
            const int to = 1000003;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
            const int numberOfMediaItemTypesWithOneMillionItems = 0; //Assuming that there is not 1000000 items of a specific type
            int numberOfKeyValuePairs = dictionary.Count;
            Assert.AreEqual(numberOfMediaItemTypesWithOneMillionItems, numberOfKeyValuePairs);
        }

        [TestMethod]
        public void FindMediaItemRange_ToExceedsNumberOfElements_ItemCount()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 100;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
            const int numberOfBooks = 4; //Assuming we have exactly 4 books 
            var bookList = dictionary[MediaItemTypeDTO.Book];
            Assert.AreEqual(numberOfBooks, bookList.MediaItemList.Count); 
        }

        [TestMethod]
        public void FindMediaItemRange_RangeTooBig()
        {
            try
            {
                var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
                const int from = 1;
                const int to = 101;
                mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("The requested range exceeds the cap of 100", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [ExpectedException(typeof(InvalidCredentialException))] //TODO Should be argument null exception
        [TestMethod]
        public void FindMediaItemRange_ClientTokenNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 10;
            mediaItemLogic.FindMediaItemRange(@from, to, null, null, null);
        }

        [ExpectedException(typeof(InvalidCredentialException))]
        [TestMethod]
        public void FindMediaItemRange_ClientTokenInvalid()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 10;
            mediaItemLogic.FindMediaItemRange(@from, to, null, null, "invalidToken");
        }

        [TestMethod]
        public void FindMediaItemRange_MediaItemTypeAndSearchKeyAreNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
            const int numberOfMediaItemTypes = 3; //Books, music, movies
            int numberOfKeyValuePairs = dictionary.Count;
            Assert.AreEqual(numberOfMediaItemTypes, numberOfKeyValuePairs); //Assuming that there is at least one media item per type
        }

        [TestMethod]
        public void FindMediaItemRange_ValidMediaItemTypeSearchKeyIsNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, MediaItemTypeDTO.Movie, null, "testClient");
            const int expectedNumberOfMovies = to - (from - 1); 
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(expectedNumberOfMovies, movieList.MediaItemList.Count);
        }

        [TestMethod]
        public void FindMediaItemRange_ValidMediaItemTypeSearchKeyIsNull_HitCount()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, MediaItemTypeDTO.Movie, null, "testClient");
            const int expectedNumberOfSearchResults = 5; 
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(expectedNumberOfSearchResults, movieList.NumberOfSearchResults);
        }

        [TestMethod]
        public void FindMediaItemRange_MediaItemTypeIsNullValidSearchKey()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, null, "Director1", "testClient");
            const int expectedNumberOfMovies = to - (from - 1); 
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(expectedNumberOfMovies, movieList.MediaItemList.Count);
        }

        [TestMethod]
        public void FindMediaItemRange_ValidMediaItemTypeValidSearchKey()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, MediaItemTypeDTO.Movie, "Director1", "testClient");
            const int numberOfMoviesThatMatchesSearchKey = 2; 
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(numberOfMoviesThatMatchesSearchKey, movieList.MediaItemList.Count);
        }

        [TestMethod]
        public void FindMediaItemRange_SearchKeyIsWhiteSpace()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(@from, to, MediaItemTypeDTO.Movie, " ", "testClient");
            const int numberOfMoviesThatMatchesSearchKey = 2; //Two movies match the search key " "
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(numberOfMoviesThatMatchesSearchKey, movieList.MediaItemList.Count);
        }
        #endregion

        #region RateMediaItem
        /* RateMediaItem
         * userId < 1
         * userId > int.MaxValue
         * mediaItemId < 1
         * mediaItemId > int.MaxValue
         * rating < 1
         * rating > 10
         * invalid clientToken
         * user never rated media item before
         * user already rated media item
         * 
         */

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_UserIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = -4;
            const int mediaItemId = 1;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_UserIdExceedsMax() //TODO Doesn't exceed max, it is exactly max.
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = int.MaxValue;
            const int mediaItemId = 1;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_MediaItemIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = -2;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_MediaItemIdExceedsMax() //TODO doesn't exceed max, it is exactly max
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = int.MaxValue;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_RatingLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = 1;
            const int rating = -2;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_RatingGreaterThanTen()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = 1;
            const int rating = 11;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(InvalidCredentialException))]
        [TestMethod]
        public void RateMediaItem_InvalidClientToken()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = 1;
            const int rating = 8;
            const string token = "invalidToken";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void RateMediaItem_ClientTokenIsNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = 1;
            const int rating = 8;
            const string token = null;
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [TestMethod]
        public void RateMediaItem_ValidNewRating()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 2;
            const int mediaItemId = 2;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
            //Assert something - but what? _dbStorage.Get<Rating>() returns the mock data
        }

        [TestMethod]
        public void RateMediaItem_ValidUpdateRating()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = 2;
            const int rating = 3; //User 1 rates media item 2 3 instead of 1
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
            //Assert something - but what? _dbStorage.Get<Rating>() returns the mock data
        }

        #endregion

        #region GetAverageRating
        /* GetAverageRating
         * mediaItemId < 1
         * mediaItemId > int.MaxValue
         * no rating
         * 1 rating
         * 3 ratings
         */

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetAverageRating_MediaItemIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.GetAverageRating(-2);
        }
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetAverageRating_MediaItemIdExceedsMax() //TODO doesn't exceed max, is exactly max
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.GetAverageRating(int.MaxValue);
        }
        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void GetAverageRating_MediaItemNotRated()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.GetAverageRating(3);
        }
        [TestMethod]
        public void GetAverageRating_OneRating()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var actual = mediaItemLogic.GetAverageRating(2);
            const double expected = 1.0;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void GetAverageRating_MultipleRatings()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var actual = mediaItemLogic.GetAverageRating(1);
            const double expected = 6.333333;
            Assert.AreEqual(expected, actual, 0.01);
        }
        #endregion
    }
}
