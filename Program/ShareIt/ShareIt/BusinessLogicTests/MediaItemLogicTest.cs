using System;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.ServiceModel;
using System.Collections.Generic;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
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
        private string _filePath = "";
        private string _invalidFilePath = @"C:\Invalid\Path.txt";

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
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i == 2), It.Is<string>(s => s == "testClient"))).Returns(true);
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i != 2), It.Is<string>(s => s != "testClient"))).Returns(false);
            _authLogic = authMoq.Object;
        }

        private void SetupDbStorageMock()
        {
            var dbMoq = new Mock<IStorageBridge>();
            dbMoq.Setup(foo => foo.Update(It.IsAny<Entity>())).Verifiable();

            var mediaItems = SetupMediaItems();
            var info = new HashSet<EntityInfo>(new EntityInfoEqualityComparer());
            foreach (var item in mediaItems)
            {
                foreach (var entityInfo in item.EntityInfo)
                {
                    info.Add(entityInfo);
                }
            }
            var ratings = SetupRatings();
            var users = SetupUsers();
            dbMoq.Setup(foo => foo.Get<UserAcc>()).Returns(users.AsQueryable);
            dbMoq.Setup(foo => foo.Get<Rating>()).Returns(ratings.AsQueryable);
            dbMoq.Setup(foo => foo.Get<EntityInfo>()).Returns(info.AsQueryable);
            dbMoq.Setup(foo => foo.Get<Entity>()).Returns(mediaItems.AsQueryable);

            //Add methods
            dbMoq.Setup(foo => foo.Add(It.IsAny<UserAcc>())).Callback(new Action<UserAcc>((e) => users.Add(e))).Verifiable();
            dbMoq.Setup(foo => foo.Add(It.IsAny<Rating>())).Callback(new Action<Rating>((e) => ratings.Add(e))).Verifiable();
            dbMoq.Setup(foo => foo.Add(It.IsAny<EntityInfo>())).Callback(new Action<EntityInfo>((e) => info.Add(e))).Verifiable();
            dbMoq.Setup(foo => foo.Add(It.IsAny<Entity>())).Callback(new Action<Entity>((e) => mediaItems.Add(e))).Verifiable();

            //Update methods
            dbMoq.Setup(foo => foo.Update(It.IsAny<Rating>())).Callback(new Action<Rating>((e) => ratings.Add(e))).Verifiable();

            //Delete methods
            dbMoq.Setup(foo => foo.Delete<Entity>(It.IsAny<int>())).Callback(new Action<int>(e => mediaItems.RemoveWhere(a => a.Id == e))).Verifiable();

            _dbStorage = dbMoq.Object;
        }

        private HashSet<Entity> SetupMediaItems()
        {

            var set = new HashSet<Entity>(new EntityEqualityComparer());

            /*//setup filepath
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("Test");
                stream.Position = 0;
                //execution
                var target = new FileStorage();
                _filePath = target.SaveMedia(stream, 1, 1, ".txt");
            }*/

            var count = 0;

            for (int i = 1; i <= 4; i++)
            {
                var book = new Entity { Id = i, TypeId = (int)MediaItemTypeDTO.Book, ClientId = 1 };
                if (i == 1){ book.FilePath = _filePath; }
                else if (i == 3) { book.FilePath = _invalidFilePath; }

                book.EntityInfo = new List<EntityInfo>
                {
                    new EntityInfo {EntityId = i, Id = ++count, EntityInfoTypeId = 1, Data = "Book" + i, Entity = book},
                    new EntityInfo { EntityId = i, Id = ++count, EntityInfoTypeId = 2, Data = "Description" + i, Entity = book}
                };
                set.Add(book);
            }
            int musicCount = 1;
            for (int i = 5; i <= 7; i++)
            {
                var music = new Entity { Id = i, TypeId = (int)MediaItemTypeDTO.Music, ClientId = 1 };
                music.EntityInfo = new List<EntityInfo> 
                {
                    new EntityInfo {EntityId = i, Id = ++count, EntityInfoTypeId = 1, Data = "Music" + musicCount++, Entity = music},
                    new EntityInfo { EntityId = i, Id = ++count, EntityInfoTypeId = 2, Data = "Description" + i, Entity = music},
                    new EntityInfo { EntityId = i, Id = ++count, EntityInfoTypeId = 12, Data = "Artist" + musicCount++, Entity = music}
                };
                set.Add(music);
            }

            for (int i = 8; i <= 12; i++)
            {
                var movie = new Entity { Id = i, TypeId = (int)MediaItemTypeDTO.Movie, ClientId = 1 };
                movie.EntityInfo = new List<EntityInfo>
                {
                    new EntityInfo {EntityId = i, Id = ++count, EntityInfoTypeId = 1, Data = "Movie" + (i-7), Entity = movie},
                    new EntityInfo {EntityId = i, Id = ++count, EntityInfoTypeId = 2, Data = "Description" + i, Entity = movie},
                    new EntityInfo {EntityId = i, Id = ++count, EntityInfoTypeId = 11, Data = "Director" + (i-7), Entity = movie}
                };
                if (i == 11) { var info = movie.EntityInfo.Where(foo => foo.Id == count).Select(foo => foo).Single(); info.Data = "Director 1"; }
                if (i == 12) { var info = movie.EntityInfo.Where(foo => foo.Id == count).Select(foo => foo).Single(); info.Data = "Director 2"; }
                set.Add(movie);
            }

            return set;
        }

        private HashSet<Rating> SetupRatings()
        {

           return new HashSet<Rating>(new RatingEqualityComparer())
            {
                new Rating { Id = 1, UserId = 1, EntityId = 1, Value = 10 },
                new Rating { Id = 2, UserId = 1, EntityId = 2, Value = 1 },
                new Rating { Id = 3, UserId = 2, EntityId = 1, Value = 7 },
                new Rating { Id = 4, UserId = 3, EntityId = 1, Value = 2 }
            };
        }

        private HashSet<UserAcc> SetupUsers()
        {

            return new HashSet<UserAcc>(new UserAccEqualityComparer())
            {
                new UserAcc {Id = 1},
                new UserAcc {Id = 2},
                new UserAcc {Id = 3},
            };
        }

        #endregion
        #region Cleanup
        [TestCleanup]
        public void CleanUp()
        {
            if(File.Exists(_filePath)) { File.Delete(_filePath); }
        }
        #endregion
        #region GetMediaItemInformation
        [ExpectedException(typeof(FaultException<MediaItemNotFound>))]
        [TestMethod]
        public void GetMediaItemInformation_InvalidMediaItemId()
        {
            const int mediaItemId = 202020;
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.GetMediaItemInformation(mediaItemId, null, "testClient");
            Assert.Fail("Expected ArgumentException");   
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
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
            var bookList = dictionary[MediaItemTypeDTO.Book];
            Assert.AreEqual(to - (from - 1), bookList.MediaItemList.Count);
        }

        [TestMethod]
        public void FindMediaItemRange_FromGreaterThanTo()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 3;
            const int to = 1;
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
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
        
        [TestMethod]
        public void FindMediaItemRange_FromExceedsNumberOfElements_ListCount()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1000000;
            const int to = 1000003;
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
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
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
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
                mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
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

        [ExpectedException(typeof(ArgumentNullException))] 
        [TestMethod]
        public void FindMediaItemRange_ClientTokenNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 10;
            mediaItemLogic.FindMediaItemRange(from, to, null, null, null);
        }

        [ExpectedException(typeof(InvalidCredentialException))]
        [TestMethod]
        public void FindMediaItemRange_ClientTokenInvalid()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 10;
            mediaItemLogic.FindMediaItemRange(from, to, null, null, "invalidToken");
        }

        [TestMethod]
        public void FindMediaItemRange_MediaItemTypeAndSearchKeyAreNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, null, null, "testClient");
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
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, MediaItemTypeDTO.Movie, null, "testClient");
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
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, MediaItemTypeDTO.Movie, null, "testClient");
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
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, null, "2", "testClient");
            const int expectedNumberOfMovies = 2; //even though range is from 1-3 there are only 2 movies matching "2"
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(expectedNumberOfMovies, movieList.MediaItemList.Count);
        }

        [TestMethod]
        public void FindMediaItemRange_ValidMediaItemTypeValidSearchKey()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int from = 1;
            const int to = 3;
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, MediaItemTypeDTO.Movie, "2", "testClient");
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
            var dictionary = mediaItemLogic.FindMediaItemRange(from, to, MediaItemTypeDTO.Movie, " ", "testClient");
            const int numberOfMoviesThatMatchesSearchKey = 2; //Two movies match the search key " "
            var movieList = dictionary[MediaItemTypeDTO.Movie];
            Assert.AreEqual(numberOfMoviesThatMatchesSearchKey, movieList.MediaItemList.Count);
        }
        #endregion
        #region RateMediaItem
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
            Assert.IsTrue(_dbStorage.Get<Rating>().Any(t => t.UserId == userId && t.EntityId == mediaItemId && t.Value == rating));


        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void RateMediaItem_UserIdNotFound()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 99; //Not existing
            const int mediaItemId = 2;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void RateMediaItem_MediaItemIdNotFound()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 2;
            const int mediaItemId = 99; //Not existing
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);
        }

        [TestMethod]
        public void RateMediaItem_ValidUpdateRating()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int userId = 1;
            const int mediaItemId = 2;
            const int rating = 3; //User 1 rates media item 2 3 instead of 1
            const string token = "testClient";
            
            Assert.IsTrue(_dbStorage.Get<Rating>().Any(t => t.Id == 2 && t.Value == 1));
            Assert.IsFalse(_dbStorage.Get<Rating>().Any(t => t.Id == 2 && t.Value == 3));
            var count = _dbStorage.Get<Rating>().Count();

            mediaItemLogic.RateMediaItem(userId, mediaItemId, rating, token);

            Assert.AreEqual(count, _dbStorage.Get<Rating>().Count());
            Assert.IsFalse(_dbStorage.Get<Rating>().Any(t => t.Id == 2 && t.Value == 1));
            Assert.IsTrue(_dbStorage.Get<Rating>().Any(t => t.Id == 2 && t.Value == 3));

        }

        #endregion
        #region GetAverageRating

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetAverageRating_MediaItemIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.GetAverageRating(-2);
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
        #region EqualityCompares
        public class RatingEqualityComparer : IEqualityComparer<Rating>
        {
            public bool Equals(Rating x, Rating y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Rating obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        public class EntityEqualityComparer : IEqualityComparer<Entity>
        {
            public bool Equals(Entity x, Entity y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Entity obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        public class EntityInfoEqualityComparer : IEqualityComparer<EntityInfo>
        {
            public bool Equals(EntityInfo x, EntityInfo y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(EntityInfo obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        public class UserAccEqualityComparer : IEqualityComparer<UserAcc>
        {
            public bool Equals(UserAcc x, UserAcc y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(UserAcc obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        #endregion
        #region DeleteMediaItem
        /* userId < 1
         * userId not existing
         * meiaItemId < 1
         * mediaItemId not existing
         * clientToken null
         * clientToken not accepted
         * admin delete
         * owner delete
         * buyer delete
         * no access delete 
         * media item filepath not found
         * media item filepath null
         * media item filepath valid
         */
        
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void DeleteMediaItem_UserIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(-2, 1, "testClient");
        }
        [ExpectedException(typeof(AccessViolationException))]
        [TestMethod]
        public void DeleteMediaItem_UserIdNotExisting()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(99, 1, "testClient");
        }
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void DeleteMediaItem_MediaItemIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(1, -2, "testClient");
        }
        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void DeleteMediaItem_MediaItemIdNotExisting()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(1, 99, "testClient");
        }
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void DeleteMediaItem_ClientTokenNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(1, 1, null);
        }
        [ExpectedException(typeof(InvalidCredentialException))]
        [TestMethod]
        public void DeleteMediaItem_ClientTokenInvalid()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(1, 1, "invalidToken");
        }
        [TestMethod]
        public void DeleteMediaItem_AdminAllowed()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);

            var dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Movie, null, "testClient");
            var countBeforeDeleting = dictionary[MediaItemTypeDTO.Movie].NumberOfSearchResults;

            mediaItemLogic.DeleteMediaItem(2, 10, "testClient"); //user 2 is admin (item 10 is movie3

            dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Movie, null, "testClient");
            var countAfterDeleting = dictionary[MediaItemTypeDTO.Movie].NumberOfSearchResults;

            Assert.AreEqual(countBeforeDeleting - 1, countAfterDeleting);
        }
        [TestMethod]
        public void DeleteMediaItem_OwnerAllowed()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);

            var dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countBeforeDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;

            mediaItemLogic.DeleteMediaItem(1, 2, "testClient"); //user 1 owns item 2 (item 2 is book2)

            dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countAfterDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;

            Assert.AreEqual(countBeforeDeleting - 1, countAfterDeleting);
        }
        [ExpectedException(typeof(AccessViolationException))]
        [TestMethod]
        public void DeleteMediaItem_BuyerNotAllowed()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(1, 3, "testClient"); //user 1 has bought item 3
        }
        [ExpectedException(typeof(AccessViolationException))]
        [TestMethod]
        public void DeleteMediaItem_NoAccessNotAllowed()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(1, 1, "testClient"); //user 1 has no access to item 1
        }

        [TestMethod]
        public void DeleteMediaItem_FilePathNotFound()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);

            var dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countBeforeDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;

            mediaItemLogic.DeleteMediaItem(2, 3, "testClient"); //user 2 is admin (item 3 is book3 which has an invalid filePath)

            dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countAfterDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;
            Assert.AreEqual(countBeforeDeleting - 1, countAfterDeleting);
            Assert.IsFalse(File.Exists(_invalidFilePath));
        }

        [TestMethod]
        public void DeleteMediaItem_ValidFilePath()
        {
            Assert.Fail("The _filePath has not been setup correctly");
            Assert.IsTrue(File.Exists(_filePath)); //Make sure the file exists before deleting the item
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);

            var dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countBeforeDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;

            mediaItemLogic.DeleteMediaItem(2, 1, "testClient"); //user 2 is admin (item 1 is book1 which has a valid filePath)

            dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countAfterDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;
            Assert.AreEqual(countBeforeDeleting - 1, countAfterDeleting);
            Assert.IsFalse(File.Exists(_filePath));
        }
        #endregion
    }
}
