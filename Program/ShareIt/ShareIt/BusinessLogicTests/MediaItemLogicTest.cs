using System;
using System.Linq;
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
                .Returns(true);
            authMoq.Setup(
                foo =>
                    foo.CheckUserExists(It.Is<UserDTO>(u => u.Password != "testPassword" && u.Username == "testUserName")))
                .Returns(false);
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
            dbMoq.Setup(foo => foo.Get<EntityInfo>()).Returns(info.AsQueryable);
            dbMoq.Setup(foo => foo.Get<Entity>()).Returns(mediaItems.AsQueryable);
            _dbStorage = dbMoq.Object;
        }

        private HashSet<Entity> SetupMediaItems()
        {
            //Add some data
            var book1 = new Entity { Id = 1, TypeId = (int)MediaItemTypeDTO.Book, ExtensionId = 1, ClientId = 1};
            book1.EntityInfo = new List<EntityInfo>
            {
                new EntityInfo {EntityId = 1, Id = 1, EntityInfoTypeId = 1, Data = "Book1"},
                new EntityInfo { EntityId = 1, Id = 2, EntityInfoTypeId = 2, Data = "Description1" }
            };
            var book2 = new Entity { Id = 2, TypeId = (int)MediaItemTypeDTO.Book, ExtensionId = 1, ClientId = 1};
            book2.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 2, Id = 3, EntityInfoTypeId = 1, Data = "Book2"},
                new EntityInfo { EntityId = 2, Id = 4, EntityInfoTypeId = 2, Data = "Description2" } 
            };
            var book3 = new Entity { Id = 3, TypeId = (int)MediaItemTypeDTO.Book, ExtensionId = 1, ClientId = 1 };
            book3.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 3, Id = 5, EntityInfoTypeId = 1, Data = "Book3"},
                new EntityInfo { EntityId = 3, Id = 6, EntityInfoTypeId = 2, Data = "Description3" } 
            };
            var book4 = new Entity { Id = 4, TypeId = (int)MediaItemTypeDTO.Book, ExtensionId = 1, ClientId = 1 };
            book4.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 4, Id = 7, EntityInfoTypeId = 1, Data = "Book4"},
                new EntityInfo { EntityId = 4, Id = 8, EntityInfoTypeId = 2, Data = "Description4" } 
            };

            var music1 = new Entity { Id = 5, TypeId = (int)MediaItemTypeDTO.Music, ExtensionId = 2, ClientId = 1 };
            music1.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 5, Id = 9, EntityInfoTypeId = 1, Data = "Music1"},
                new EntityInfo { EntityId = 5, Id = 10, EntityInfoTypeId = 2, Data = "Description5" },
                new EntityInfo { EntityId = 5, Id = 11, EntityInfoTypeId = 12, Data = "Artist1" }
            };
            var music2 = new Entity { Id = 6, TypeId = (int)MediaItemTypeDTO.Music, ExtensionId = 2, ClientId = 1 };
            music2.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 6, Id = 12, EntityInfoTypeId = 1, Data = "Music2"},
                new EntityInfo {EntityId = 6, Id = 13, EntityInfoTypeId = 2, Data = "Description6"},
                new EntityInfo {EntityId = 6, Id = 14, EntityInfoTypeId = 12, Data = "Artist2"}
            };
            var music3 = new Entity { Id = 7, TypeId = (int)MediaItemTypeDTO.Music, ExtensionId = 2, ClientId = 1 };
            music3.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 7, Id = 15, EntityInfoTypeId = 1, Data = "Music3"},
                new EntityInfo {EntityId = 7, Id = 16, EntityInfoTypeId = 2, Data = "Description7"},
                new EntityInfo {EntityId = 7, Id = 17, EntityInfoTypeId = 12, Data = "Artist3"}
            };

            var movie1 = new Entity { Id = 8, TypeId = (int)MediaItemTypeDTO.Movie, ExtensionId = 3, ClientId = 1 };
            movie1.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 8, Id = 18, EntityInfoTypeId = 1, Data = "Movie1"},
                new EntityInfo {EntityId = 8, Id = 19, EntityInfoTypeId = 2, Data = "Description8"},
                new EntityInfo {EntityId = 8, Id = 20, EntityInfoTypeId = 11, Data = "Director1"}
            };
            var movie2 = new Entity { Id = 9, TypeId = (int)MediaItemTypeDTO.Movie, ExtensionId = 3, ClientId = 1 };
            movie2.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 9, Id = 21, EntityInfoTypeId = 1, Data = "Movie2"},
                new EntityInfo {EntityId = 9, Id = 22, EntityInfoTypeId = 2, Data = "Description9"},
                new EntityInfo {EntityId = 9, Id = 23, EntityInfoTypeId = 11, Data = "Director2"}
            };
            var movie3 = new Entity { Id = 10, TypeId = (int)MediaItemTypeDTO.Movie, ExtensionId = 3, ClientId = 1 };
            movie3.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 10, Id = 24, EntityInfoTypeId = 1, Data = "Movie3"},
                new EntityInfo {EntityId = 10, Id = 25, EntityInfoTypeId = 2, Data = "Description10"},
                new EntityInfo {EntityId = 10, Id = 26, EntityInfoTypeId = 11, Data = "Director3"}
            };
            var movie4 = new Entity { Id = 11, TypeId = (int)MediaItemTypeDTO.Movie, ExtensionId = 3, ClientId = 1 };
            movie4.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 11, Id = 27, EntityInfoTypeId = 1, Data = "Movie4"},
                new EntityInfo {EntityId = 11, Id = 28, EntityInfoTypeId = 2, Data = "Description11"},
                new EntityInfo {EntityId = 11, Id = 29, EntityInfoTypeId = 11, Data = "Director 1"}
            };
            var movie5 = new Entity { Id = 12, TypeId = (int)MediaItemTypeDTO.Movie, ExtensionId = 3, ClientId = 1 };
            movie5.EntityInfo = new List<EntityInfo> {
                new EntityInfo {EntityId = 12, Id = 30, EntityInfoTypeId = 1, Data = "Movie5"},
                new EntityInfo {EntityId = 12, Id = 31, EntityInfoTypeId = 2, Data = "Description12"},
                new EntityInfo {EntityId = 12, Id = 32, EntityInfoTypeId = 11, Data = "Director 2"}
            };

            return new HashSet<Entity>{book1, book2, book3, book4, music1, music2, music3, movie1, movie2, movie3, movie4, movie5};
        }

        #endregion

        #region GetMediaItemInformation
        [TestMethod]
        public void GetMediaItemInformation_InvalidMediaItemId()
        {
            const int mediaItemId = 2;

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
            var mediaItemId = 1;

            MediaItemDTO m = mediaItemLogic.GetMediaItemInformation(mediaItemId, null, "testClient");

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
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var mediaItemId = 1;

            MediaItemDTO m = mediaItemLogic.GetMediaItemInformation(mediaItemId, null, "testClient");

            var list = new List<InformationTypeDTO>();

            foreach (var info in m.Information)
            {
                list.Add(info.Type);
            }

            Assert.AreEqual(list[0], InformationTypeDTO.Language);
            Assert.AreEqual(list[1], InformationTypeDTO.Price);
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
            Assert.AreEqual(to - (from - 1), bookList.MediaItemList.Count);
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

        [TestMethod]
        public void FindMediaItemRange_ToIsNull()
        {
            try
            {
                var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
                const int from = 1;
                const int to = 0;
                mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("\"from\" and \"to\" must be >= 1", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void FindMediaItemRange_FromIsNull()
        {
            try
            {
                var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
                const int from = 0;
                const int to = 3;
                mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("\"from\" and \"to\" must be >= 1", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
        }

        [TestMethod]
        public void FindMediaItemRange_FromAndToAreNull()
        {
            try
            {
                var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
                const int from = 0;
                const int to = 0;
                mediaItemLogic.FindMediaItemRange(@from, to, null, null, "testClient");
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("\"from\" and \"to\" must be >= 1", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected ArgumentException");
            }
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

        [ExpectedException(typeof(InvalidCredentialException))]
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
    }
}
