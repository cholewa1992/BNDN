using System;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntergrationTest
{
    [TestClass]
    public class AccessRightLogic
    {

        private readonly UserDTO _jacob = new UserDTO { Username = "Jacob", Password = "1234" };
        private readonly UserDTO _loh = new UserDTO { Username = "Loh", Password = "2143" };
        private readonly UserDTO _mathias = new UserDTO { Username = "Mathias", Password = "4321" };
        private readonly string _smu = Properties.Resources.SMU;
        private readonly string _artShare = Properties.Resources.ArtShare;

        #region Setup & TearDown
        [TestInitialize]
        public void Setup()
        {
            using (var db = new RentIt08Entities())
            {
                TearDown();
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
            }
        }

       public void TearDown()
        {
            using (var db = new RentIt08Entities())
            {
                db.Database.ExecuteSqlCommand(Properties.Resources.datamodel); 
            }
        }
        #endregion

        [TestMethod]
        public void IntegrationTest_AccessRightLogic_MakeAdmin()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();
            var al = blf.CreateAuthLogic();
            
            Assert.IsFalse(al.IsUserAdminOnClient(_mathias, _artShare));
            Assert.IsFalse(al.IsUserAdminOnClient(_jacob, _smu));

            arl.MakeAdmin(_jacob, 3, _artShare); //Making Mathias admin of ArtShare
            arl.MakeAdmin(_loh, 1, _smu); //Making Jacob admin of SMU client

            Assert.IsTrue(al.IsUserAdminOnClient(_jacob, _artShare));
            Assert.IsTrue(al.IsUserAdminOnClient(_loh, _smu));
        }

        [TestMethod]
        public void IntegrationTest_AccessRightLogic_DeleteAccessRight()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();
            var mil = blf.CreateMediaItemLogic();
            var ul = blf.CreateUserLogic();
            var al = blf.CreateAuthLogic();
            var accountInfo = ul.GetAccountInformation(new UserDTO {Username = "Loh", Password = "2143"}, 2, "4321");
            
            arl.DeleteAccessRight(new UserDTO {Username = "Loh", Password = "2143"}, 2, "4321"); //Demoting Loh from admin

            //TODO CANNOT DELETE ACCESS RIGHT
        }

        [TestMethod]
        public void IntegrationTest_AccessRightLogic_Purchase()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();
            arl.Purchase(_mathias, 1, DateTime.Now.AddDays(1), _artShare);
        }

        [TestMethod]
        public void IntegrationTest_AccessRightLogic_EditExpiration()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var arl = blf.CreateAccessRightLogic();
            //TODO Still don't have AccessRightId
        }

        [TestMethod]
        public void IntegrationTest_AuthLogic_EditExpiration()
        {
            var blf = BusinessLogicFacade.GetBusinessFactory();
            var al = blf.CreateAuthLogic();
            
        }


    }
}
