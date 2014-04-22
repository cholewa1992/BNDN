using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessRightType = BusinessLogicLayer.AccessRightType;

namespace IntergrationTest
{
    [TestClass]
    public class BuinessLogicDalIntegrationTest
    {
        #region Setup
        private readonly UserDTO _jacob = new UserDTO { Username = "Jacob", Password = "1234" };
        private readonly UserDTO _loh = new UserDTO { Username = "Loh", Password = "2143" };
        private readonly UserDTO _mathias = new UserDTO { Username = "Mathias", Password = "4321" };
        private readonly string _smu = Properties.Resources.SMU;
        private readonly string _artShare = Properties.Resources.ArtShare;

        [TestInitialize]
        public void Setup()
        {
            using (var db = new RentIt08Entities())
            {
                db.Database.ExecuteSqlCommand(Properties.Resources.datamodel);
                #region AccessRightType
                db.Database.ExecuteSqlCommand("INSERT INTO AccessRightType (Name) VALUES  ('Buyer')");
                db.Database.ExecuteSqlCommand("INSERT INTO AccessRightType (Name) VALUES  ('Owner')");
                #endregion
                #region UserInfoType
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfoType (Type) VALUES  ('Email')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfoType (Type) VALUES  ('Firstname')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfoType (Type) VALUES  ('Lastname')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfoType (Type) VALUES  ('Location')");
                #endregion
                #region EntityInfoType
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Title')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Description')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Price')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Picture')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('KeywordTag')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Genre')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('TrackLength')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Runtime')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('NumberOfPages')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Author')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Director')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Artist')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('CastMember')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('ReleaseDate')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Language')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('ExpirationDate')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('AverageRating')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfoType (Name) VALUES  ('Thumbnail')");
                #endregion
                #region Client
                db.Database.ExecuteSqlCommand("INSERT INTO Client (Name,Token) VALUES  ('ArtShare', '7dac496c534911c0ef47bce1de772502b0d6a6c60b1dbd73c1d3f285f36a0f61')");
                db.Database.ExecuteSqlCommand("INSERT INTO Client (Name,Token) VALUES  ('SMU', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855')");
                #endregion
                #region UsersAcc
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Jacob', '1234')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Loh', '2143')");
                db.Database.ExecuteSqlCommand("INSERT INTO UserAcc (Username, Password) VALUES  ('Mathias', '4321')");
                #endregion
                #region UserInfo
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('jacob@cholewa.dk', 1, 1)");
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('Jacob', 1, 2)");
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('Cholewa', 1, 3)");
                db.Database.ExecuteSqlCommand("INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('Denmark', 1, 4)");
                #endregion
                #region ClientAdmin
                db.Database.ExecuteSqlCommand("INSERT INTO ClientAdmin (ClientId, UserId) VALUES  (1, 1)");
                db.Database.ExecuteSqlCommand("INSERT INTO ClientAdmin (ClientId, UserId) VALUES  (2, 2)");
                #endregion
                #region EntityType
                db.Database.ExecuteSqlCommand("INSERT INTO EntityType (Type) VALUES  ('Movie')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityType (Type) VALUES  ('Book')");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityType (Type) VALUES  ('Music')");
                #endregion
                #region Entity
                db.Database.ExecuteSqlCommand("INSERT INTO Entity (FilePath, ClientId, TypeId) VALUES  ('C:/lol', 1, 1)");
                #endregion
                #region AccessRight

                db.Entry( new AccessRight
                {
                    AccessRightTypeId = (int) AccessRightType.Buyer,
                    EntityId = 1,
                    Expiration = DateTime.Now.AddDays(2),
                    UserId = 3
                }).State = EntityState.Added;

                db.Entry(new AccessRight
                {
                    AccessRightTypeId = 2,
                    EntityId = 1,
                    UserId = 1,
                }).State = EntityState.Added;

                db.SaveChanges();


                #endregion
                #region EntityInfo
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfo (Data, EntityId , EntityInfoTypeId) VALUES  ('Lord of the Rings', 1, 1)");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfo (Data, EntityId , EntityInfoTypeId) VALUES  ('A movie about a ring', 1, 2)");
                db.Database.ExecuteSqlCommand("INSERT INTO EntityInfo (Data, EntityId , EntityInfoTypeId) VALUES  ('$1000', 1, 3)");
                #endregion
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            if (File.Exists("files/user_3/2.m4a")) File.Delete("files/user_3/2.m4a");
            if (File.Exists("img/thumbnail_1.jpg")) File.Delete("img/thumbnail_1.jpg");
        }

        #endregion
        #region Access Right Logic Tests
        [TestMethod]
        public void IntegrationTest_AccessRightLogic_MakeAdmin()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();
            var al = blf.CreateAuthLogic();
            
            Assert.IsFalse(al.IsUserAdminOnClient(_mathias, _artShare));
            Assert.IsFalse(al.IsUserAdminOnClient(_jacob, _smu));

            arl.MakeAdmin(_jacob, 3, _artShare); //Making Jacob admin of ArtShare
            arl.MakeAdmin(_loh, 1, _smu); //Making Loh admin of SMU client

            Assert.IsTrue(al.IsUserAdminOnClient(_jacob, _artShare));
            Assert.IsTrue(al.IsUserAdminOnClient(_loh, _smu));
        }

        [TestMethod]
        public void IntegrationTest_AccessRightLogic_DeleteAccessRight()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();

            using (var db = new RentIt08Entities())
            {
                Assert.IsTrue(db.AccessRight.Any(t => t.UserId == 3));
            }

            var history = arl.GetPurchaseHistory(_mathias, 3, _artShare);
            arl.DeleteAccessRight(_jacob, history.Single().Id, _artShare);

            using (var db = new RentIt08Entities())
            {
                Assert.IsFalse(db.AccessRight.Any(t => t.UserId == 3));
            }
        }

        [TestMethod]
        public void IntegrationTest_AccessRightLogic_Purchase()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();
            arl.Purchase(_mathias, 1, DateTime.Now.AddDays(1), _artShare);

            using (var db = new RentIt08Entities())
            {
                Assert.IsTrue(db.AccessRight.Any(t => t.EntityId == 1 && t.AccessRightTypeId == (int) BusinessLogicLayer.AccessRightType.Buyer && t.UserId == 3));
            }
        }

        [TestMethod]
        public void IntegrationTest_AccessRightLogic_EditExpiration()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();

            DateTime? dt;

            using (var db = new RentIt08Entities())
            {
                dt = db.AccessRight.Single(t => t.UserId == 3).Expiration;
            }

            var history = arl.GetPurchaseHistory(_mathias, 3, _artShare);
            var ar = history.Single();
            ar.Expiration = DateTime.Now.AddDays(10);

            arl.EditExpiration(_jacob, ar, _artShare);

            using (var db = new RentIt08Entities())
            {
                var newDt = db.AccessRight.Single(t => t.UserId == 3).Expiration;
                Assert.AreNotEqual(dt,newDt);
            }
        }
        #endregion
        #region Auth logic tests
        [TestMethod]
        public void IntegrationTest_AuthLogic_CheckClientExists()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var al = blf.CreateAuthLogic();
            Assert.IsTrue(al.CheckClientExists(new ClientDTO{Name = "SMU", Token = _smu}));
            Assert.IsFalse(al.CheckClientExists(new ClientDTO{Name = "SMU", Token = _artShare}));
            Assert.IsFalse(al.CheckClientExists(new ClientDTO{Name = "hello", Token = _smu}));
            Assert.IsFalse(al.CheckClientExists(new ClientDTO{Name = "hello", Token = "cool"}));
        }
        [TestMethod]
        public void IntegrationTest_AuthLogic_CheckUserExists()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var al = blf.CreateAuthLogic();
            Assert.AreEqual(1, al.CheckUserExists(_jacob, _artShare));
            Assert.AreEqual(1, al.CheckUserExists(_jacob, _smu));
            Assert.AreEqual(-1, al.CheckUserExists(new UserDTO{Username = "Thomas", Password = "52234"}, _artShare));
        }

        [TestMethod]
        public void IntegrationTest_AuthLogic_IsUserAdminOnClient()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var al = blf.CreateAuthLogic();
            Assert.IsTrue(al.IsUserAdminOnClient(_jacob, _artShare));
            Assert.IsFalse(al.IsUserAdminOnClient(_jacob, _smu));
            Assert.IsFalse(al.IsUserAdminOnClient(_loh, _artShare));
            Assert.IsTrue(al.IsUserAdminOnClient(_loh, _smu));
            Assert.IsFalse(al.IsUserAdminOnClient(_mathias, _artShare));
            Assert.IsFalse(al.IsUserAdminOnClient(_mathias, _smu));
        }
        #endregion
        #region User logic Tests
        [TestMethod]
        public void IntegrationTest_UserLogic_CreateAccount()
        {
            using (var db = new RentIt08Entities())
            {
                Assert.IsFalse(db.UserAcc.Any(t => t.Username == "Thomas" && t.Password == "3421"));
            }

            var blf = BusinessLogicFacade.GetBusinessFactory();
            var ul = blf.CreateUserLogic();
            ul.CreateAccount(new UserDTO {Username = "Thomas", Password = "3421"}, _artShare);

            using (var db = new RentIt08Entities())
            {
                Assert.IsTrue(db.UserAcc.Any(t => t.Username == "Thomas" && t.Password == "3421"));
            }
        }

        [TestMethod]
        public void IntegrationTest_UserLogic_DeleteUser()
        {
            using (var db = new RentIt08Entities())
            {
                Assert.IsTrue(db.UserAcc.Any(t => t.Username == _mathias.Username && t.Password == _mathias.Password));
            }

            var blf = BusinessLogicFacade.GetBusinessFactory();
            var ul = blf.CreateUserLogic();
            ul.DeleteUser(_jacob, 3, _artShare);

            using (var db = new RentIt08Entities())
            {
                Assert.IsFalse(db.UserAcc.Any(t => t.Username == _mathias.Username && t.Password == _mathias.Password));
            }
        }

        [TestMethod]
        public void IntegrationTest_UserLogic_GetAccountInformation()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var ul = blf.CreateUserLogic();
            var info = ul.GetAccountInformation(_jacob, 1, _artShare);

            var email = info.Information.Single(t => t.Type == UserInformationTypeDTO.Email);
            var first = info.Information.Single(t => t.Type == UserInformationTypeDTO.Firstname);
            var last = info.Information.Single(t => t.Type == UserInformationTypeDTO.Lastname);
            var location = info.Information.Single(t => t.Type == UserInformationTypeDTO.Location);

            Assert.AreEqual("jacob@cholewa.dk", email.Data);
            Assert.AreEqual("Jacob", first.Data);
            Assert.AreEqual("Cholewa", last.Data);
            Assert.AreEqual("Denmark", location.Data);

        } 

        [TestMethod]
        public void IntegrationTest_UserLogic_UpdateAccountInformation()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var ul = blf.CreateUserLogic();

            var newUser = new UserDTO
            {
                Username = _jacob.Username,
                Password = _jacob.Password,
                Information = new List<UserInformationDTO>
                {
                    new UserInformationDTO {Data = "jbec@itu.dk", Type = UserInformationTypeDTO.Email},
                    new UserInformationDTO {Data = "Jacob", Type = UserInformationTypeDTO.Firstname},
                    new UserInformationDTO {Data = "Cholewa", Type = UserInformationTypeDTO.Lastname},
                    new UserInformationDTO {Data = "Denmark", Type = UserInformationTypeDTO.Location}
                }
            };

            ul.UpdateAccountInformation(_jacob, newUser, _artShare);
            var info = ul.GetAccountInformation(_jacob, 1, _artShare);

            var email = info.Information.Single(t => t.Type == UserInformationTypeDTO.Email);
            var first = info.Information.Single(t => t.Type == UserInformationTypeDTO.Firstname);
            var last = info.Information.Single(t => t.Type == UserInformationTypeDTO.Lastname);
            var location = info.Information.Single(t => t.Type == UserInformationTypeDTO.Location);

            Assert.AreEqual("jbec@itu.dk", email.Data);
            Assert.AreEqual("Jacob", first.Data);
            Assert.AreEqual("Cholewa", last.Data);
            Assert.AreEqual("Denmark", location.Data);

        }
        #endregion
        #region Media Item Logic Tests
        [TestMethod]
        public void IntegrationTest_MediaItemLogic_RateMediaItem()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var mil = blf.CreateMediaItemLogic();

            using (var db = new RentIt08Entities())
            {
                Assert.IsFalse(db.Rating.Any());
            }

            mil.RateMediaItem(_mathias, 1, 5, _artShare);

            using (var db = new RentIt08Entities())
            {
                Assert.IsTrue(db.Rating.Any());
            }
        } 

        [TestMethod]
        public void IntegrationTest_MediaItemLogic_DeleteMediaItem()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var mil = blf.CreateMediaItemLogic();

            using (var db = new RentIt08Entities())
            {
                Assert.IsTrue(db.Entity.Any());
            }

            mil.DeleteMediaItem(_jacob, 1, _artShare);

            using (var db = new RentIt08Entities())
            {
                Assert.IsFalse(db.Entity.Any());
            }
        }
        
        [TestMethod]
        public void IntegrationTest_MediaItemLogic_GetMediaItemInformation()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var mil = blf.CreateMediaItemLogic();
            var result = mil.GetMediaItemInformation(1, _mathias, _artShare);
            Assert.AreEqual("Lord of the Rings" ,result.Information.Single(t => t.Type == InformationTypeDTO.Title).Data);
            Assert.AreEqual("A movie about a ring" ,result.Information.Single(t => t.Type == InformationTypeDTO.Description).Data);
            Assert.AreEqual("$1000" ,result.Information.Single(t => t.Type == InformationTypeDTO.Price).Data);
        }
        
        [TestMethod]
        public void IntegrationTest_MediaItemLogic_FindMediaItemRange()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var mil = blf.CreateMediaItemLogic();

            mil.FindMediaItemRange(1,3, null, null, _artShare);

            var result = mil.FindMediaItemRange(1, 1, null, "Lord of the Rings", _artShare);
            Assert.AreEqual(1, result.Single().Value.MediaItemList.Single().Id);
        }

        [TestMethod]
        public void IntegrationTest_MediaItemLogic_UpdateMediaItem()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var mil = blf.CreateMediaItemLogic();

            var result = mil.FindMediaItemRange(1, 1, null, "Lord of the Rings", _artShare);
            Assert.AreEqual(1, result.Single().Value.MediaItemList.Single().Id);

            var mediaItem = result.Single().Value.MediaItemList.Single();
            mediaItem.Information.Single(t => t.Type == InformationTypeDTO.Title).Data = "Lord of the Rings two";

            mil.UpdateMediaItem(_mathias, mediaItem, _artShare);

            result = mil.FindMediaItemRange(1, 1, null, "Lord of the Rings two", _artShare);
            Assert.AreEqual(1, result.Single().Value.MediaItemList.Single().Id);
        }
        #endregion
        #region Data Transfer Logic tests

        [TestMethod]
        public void IntegrationTest_DataTransferLogic_GetMediaStream()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var dtl = blf.CreateDataTransferLogic();

            Assert.IsFalse(File.Exists("files/user_3/2.m4a"));

            dtl.SaveMedia(_artShare, _mathias, new MediaItemDTO
            {
                Type = MediaItemTypeDTO.Music,
                FileExtension = ".m4a",
                Information = new List<MediaItemInformationDTO>
                {
                    new MediaItemInformationDTO
                    {
                        Data = "Technologic",
                        Type = InformationTypeDTO.Title
                    },
                    new MediaItemInformationDTO
                    {
                        Data = "Daft Punk",
                        Type = InformationTypeDTO.Artist
                    }
                }
            }, new MemoryStream(Properties.Resources.Technologic));

            Assert.IsTrue(File.Exists("files/user_3/2.m4a"));
        }

        
        [TestMethod]
        public void IntegrationTest_DataTransferLogic_SaveThumbnail()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var dtl = blf.CreateDataTransferLogic();
            Assert.IsFalse(File.Exists("img/thumbnail_1.jpg"));
            dtl.SaveThumbnail(_artShare, _jacob, 1, ".jpg",
                new MemoryStream(Properties.Resources.Daft_Punk_Technologic));
            Assert.IsTrue(File.Exists("img/thumbnail_1.jpg"));
        }
        #endregion
    }
}
