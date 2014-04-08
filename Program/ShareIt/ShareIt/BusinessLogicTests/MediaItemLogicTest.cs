using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.ServiceModel;
using System.Collections.Generic;
using System.Text;
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
        private string _directoryPath = @"C:\RentItUnitTest";
        private string _filePath = @"unittest.txt";
        static private string _thumbnailName = @"thumbnail_1.jpg";
        private string _thumbnailWebPath = Path.Combine("http://rentit.itu.dk/rentit08/img/", _thumbnailName);
        private string _invalidFilePath = @"C:\Invalid\Path.txt";
        private UserDTO user1 = new UserDTO{Id = 1, Username = "user1", Password = "pass1"};
        private UserDTO user2 = new UserDTO {Id = 2, Username = "user2", Password = "pass2"};

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
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password == "pass1" && u.Username == "user1"))).Returns(1);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password == "pass2" && u.Username == "user2"))).Returns(2);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password != "pass1" && u.Username == "user1"))).Returns(-1);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Password != "pass2" && u.Username == "user2"))).Returns(-1);
            authMoq.Setup(foo => foo.CheckUserExists(
                It.Is<UserDTO>(u => u.Username != "user1" && u.Username != "user2"))).Returns(-1);
            //setup checkUserAccess
            authMoq.Setup(foo => foo.CheckUserAccess(1, 1)).Returns(BusinessLogicLayer.AccessRightType.NoAccess);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 2)).Returns(BusinessLogicLayer.AccessRightType.Owner);
            authMoq.Setup(foo => foo.CheckUserAccess(1, 3)).Returns(BusinessLogicLayer.AccessRightType.Buyer);
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i == 2), It.Is<string>(s => s == "testClient"))).Returns(true);
            authMoq.Setup(foo => foo.IsUserAdminOnClient(It.Is<int>(i => i != 2), It.Is<string>(s => s != "testClient"))).Returns(false);
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(1, 1)).Returns((DateTime?) null);
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(1, 2)).Returns((DateTime?) null);
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(2, 2)).Returns(new DateTime(2015, 01, 01, 00, 00, 00));
            authMoq.Setup(foo => foo.GetBuyerExpirationDate(1, 3)).Throws<InstanceNotFoundException>();
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

            // Create the directory for the file
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);
            // Create a file. 
            using (FileStream fs = File.Create(Path.Combine(_directoryPath, _filePath)))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
            // Create a thumbnail file
            using (FileStream fs = File.Create(Path.Combine(_directoryPath, _thumbnailName)))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            var count = 0;

            for (int i = 1; i <= 4; i++)
            {
                var book = new Entity { Id = i, TypeId = (int)MediaItemTypeDTO.Book, ClientId = 1 };
                
                book.EntityInfo = new List<EntityInfo>
                {
                    new EntityInfo {EntityId = i, Id = ++count, EntityInfoTypeId = 1, Data = "Book" + i, Entity = book},
                    new EntityInfo { EntityId = i, Id = ++count, EntityInfoTypeId = 2, Data = "Description" + i, Entity = book}
                };

                if (i == 1) { book.FilePath = Path.Combine(_directoryPath, _filePath); }
                else if (i == 3) { 
                    book.FilePath = _invalidFilePath; 
                    //TODO Test if thumbnail is removed when deleting media item
                    /* NOT IMPLEMENTED YET
                    book.EntityInfo.Add(new EntityInfo
                    {
                        EntityId = i, Id = ++count, EntityInfoTypeId = 16, Data = _thumbnailWebPath, Entity = book
                    }); */
                }
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
                new UserAcc {Id = 1, Username = "user1", Password = "pass1"},
                new UserAcc {Id = 2, Username = "user2", Password = "pass2"},
                new UserAcc {Id = 3}
            };
        }
        #endregion
        #region Cleanup
        [TestCleanup]
        public void CleanUp()
        {
            if(File.Exists(Path.Combine(_directoryPath, _filePath))) { File.Delete(Path.Combine(_directoryPath, _filePath)); }
            if (File.Exists(Path.Combine(_directoryPath, _thumbnailName))) { File.Delete(Path.Combine(_directoryPath, _thumbnailName)); }
            if(Directory.Exists(_directoryPath)) { Directory.Delete(_directoryPath); }
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

        [TestMethod]
        public void GetMediaItemInformation_AccessRightOwner()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var mediaItem = mediaItemLogic.GetMediaItemInformation(1, user2, "testClient");
            var accessRightInfo = mediaItem.ExpirationDate;
            Assert.AreEqual(null, accessRightInfo);
        }
        [TestMethod]
        public void GetMediaItemInformation_AccessRightBuyer()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var mediaItem = mediaItemLogic.GetMediaItemInformation(2, user2, "testClient");
            var expected = new DateTime(2015, 01, 01, 00, 00, 00);
            var accessRightInfo = mediaItem.ExpirationDate;
            Assert.AreEqual(expected, accessRightInfo);
        }
        [TestMethod]
        public void GetMediaItemInformation_AccessRightNoAccess()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var mediaItem = mediaItemLogic.GetMediaItemInformation(3, user1, "testClient");
            Assert.IsNull(mediaItem.ExpirationDate); //TODO Note that Expiration date is null when there is no access AND when access right never expires
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
        [ExpectedException(typeof(FaultException<UnauthorizedUser>))]
        [TestMethod]
        public void RateMediaItem_UserIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            UserDTO user = new UserDTO {Id = -4};
            const int mediaItemId = 1;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(user, mediaItemId, rating, token);
        }


        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_MediaItemIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = -2;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(user1, mediaItemId, rating, token);
        }


        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_RatingLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = 1;
            const int rating = -2;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(user1, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RateMediaItem_RatingGreaterThanTen()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = 1;
            const int rating = 11;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(user1, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(InvalidCredentialException))]
        [TestMethod]
        public void RateMediaItem_InvalidClientToken()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = 1;
            const int rating = 8;
            const string token = "invalidToken";
            mediaItemLogic.RateMediaItem(user1, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void RateMediaItem_ClientTokenIsNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = 1;
            const int rating = 8;
            const string token = null;
            mediaItemLogic.RateMediaItem(user1, mediaItemId, rating, token);
        }

        [TestMethod]
        public void RateMediaItem_ValidNewRating()
        {
            
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = 2;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(user2, mediaItemId, rating, token);
            Assert.IsTrue(_dbStorage.Get<Rating>().Any(t => t.UserId == user2.Id && t.EntityId == mediaItemId && t.Value == rating));


        }

        [ExpectedException(typeof(FaultException<UnauthorizedUser>))]
        [TestMethod]
        public void RateMediaItem_UserIdNotFound()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            UserDTO user = new UserDTO { Id = 99 };
            const int mediaItemId = 2;
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(user, mediaItemId, rating, token);
        }

        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void RateMediaItem_MediaItemIdNotFound()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = 99; //Not existing
            const int rating = 8;
            const string token = "testClient";
            mediaItemLogic.RateMediaItem(user2, mediaItemId, rating, token);
        }

        [TestMethod]
        public void RateMediaItem_ValidUpdateRating()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            const int mediaItemId = 2;
            const int rating = 3; //User 1 rates media item 2 3 instead of 1
            const string token = "testClient";
            
            Assert.IsTrue(_dbStorage.Get<Rating>().Any(t => t.Id == 2 && t.Value == 1));
            Assert.IsFalse(_dbStorage.Get<Rating>().Any(t => t.Id == 2 && t.Value == 3));
            var count = _dbStorage.Get<Rating>().Count();

            mediaItemLogic.RateMediaItem(user1, mediaItemId, rating, token);

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
        public void GetAverageRating_OneRatingCheckAvg()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var dict = mediaItemLogic.GetAverageRating(2);
            double actual = 0.0;
            foreach (var entry in dict)
            {
                actual = entry.Key;
            }
            Assert.AreEqual(1.0, actual);
        }
        [TestMethod]
        public void GetAverageRating_OneRatingCheckCount()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var dict = mediaItemLogic.GetAverageRating(2);
            int actual = 0;
            foreach (var entry in dict)
            {
                actual = entry.Value;
            }
            Assert.AreEqual(1, actual);
        }
        [TestMethod]
        public void GetAverageRating_MultipleRatingsCheckAvg()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var dict = mediaItemLogic.GetAverageRating(1);
            double actual = 0.0;
            foreach (var entry in dict)
            {
                actual = entry.Key;
            }
            const double expected = 6.333333;
            Assert.AreEqual(expected, actual, 0.01);
        }
        [TestMethod]
        public void GetNumberOfRatings_MultipleRatingsCheckCount()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            var dict = mediaItemLogic.GetAverageRating(1);
            int actual = 0;
            foreach (var entry in dict)
            {
                actual = entry.Value;
            }
            Assert.AreEqual(3, actual);
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

        [ExpectedException(typeof(FaultException<UnauthorizedUser>))]
        [TestMethod]
        public void DeleteMediaItem_UserIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(new UserDTO{Id = -2}, 1, "testClient");
        }
        [ExpectedException(typeof(FaultException<UnauthorizedUser>))]
        [TestMethod]
        public void DeleteMediaItem_UserNotExisting()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(new UserDTO{Id = 99}, 1, "testClient");
        }
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void DeleteMediaItem_MediaItemIdLessThanOne()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(user1, -2, "testClient");
        }
        [ExpectedException(typeof(InstanceNotFoundException))]
        [TestMethod]
        public void DeleteMediaItem_MediaItemIdNotExisting()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(user1, 99, "testClient");
        }
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void DeleteMediaItem_ClientTokenNull()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(user1, 1, null);
        }
        [ExpectedException(typeof(InvalidCredentialException))]
        [TestMethod]
        public void DeleteMediaItem_ClientTokenInvalid()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(user1, 1, "invalidToken");
        }
        [TestMethod]
        public void DeleteMediaItem_AdminAllowed()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);

            var dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Movie, null, "testClient");
            var countBeforeDeleting = dictionary[MediaItemTypeDTO.Movie].NumberOfSearchResults;

            mediaItemLogic.DeleteMediaItem(user2, 10, "testClient"); //user 2 is admin (item 10 is movie3

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

            mediaItemLogic.DeleteMediaItem(user1, 2, "testClient"); //user 1 owns item 2 (item 2 is book2)

            dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countAfterDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;

            Assert.AreEqual(countBeforeDeleting - 1, countAfterDeleting);
        }
        [ExpectedException(typeof(UnauthorizedAccessException))]
        [TestMethod]
        public void DeleteMediaItem_BuyerNotAllowed()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(user1, 3, "testClient"); //user 1 has bought item 3
        }
        [ExpectedException(typeof(UnauthorizedAccessException))]
        [TestMethod]
        public void DeleteMediaItem_NoAccessNotAllowed()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(user1, 1, "testClient"); //user 1 has no access to item 1
        }

        [TestMethod]
        public void DeleteMediaItem_FilePathNotFound()
        {
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);

            var dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countBeforeDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;

            mediaItemLogic.DeleteMediaItem(user2, 3, "testClient"); //user 2 is admin (item 3 is book3 which has an invalid filePath)

            dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countAfterDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;
            Assert.AreEqual(countBeforeDeleting - 1, countAfterDeleting);
            Assert.IsFalse(File.Exists(_invalidFilePath));
        }

        [TestMethod]
        public void DeleteMediaItem_ValidFilePath()
        {
            Assert.IsTrue(File.Exists(Path.Combine(_directoryPath, _filePath))); //Make sure the file exists before deleting the item
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);

            var dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countBeforeDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;

            mediaItemLogic.DeleteMediaItem(user2, 1, "testClient"); //user 2 is admin (item 1 is book1 which has a valid filePath)

            dictionary = mediaItemLogic.FindMediaItemRange(1, 99, MediaItemTypeDTO.Book, null, "testClient");
            var countAfterDeleting = dictionary[MediaItemTypeDTO.Book].NumberOfSearchResults;
            Assert.AreEqual(countBeforeDeleting - 1, countAfterDeleting);
            Assert.IsFalse(File.Exists(_filePath));
        }
        //TODO Test if thumbnail is removed when deleting media item
        /* NOT IMPLEMENTED YET - FileStorage _physicalPath is null when unit testing
        [TestMethod]
        public void DeleteMediaItem_ValidThumbnail()
        {
            Assert.IsTrue(File.Exists(Path.Combine(_directoryPath, _thumbnailName)));
            var mediaItemLogic = new MediaItemLogic(_dbStorage, _authLogic);
            mediaItemLogic.DeleteMediaItem(user2, 3, "testClient"); 
            Assert.IsFalse(File.Exists(Path.Combine(_directoryPath, _thumbnailName)));
        }*/
        #endregion
    }
}
