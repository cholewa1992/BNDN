using System.IO;
using System.Web.Mvc;
using ArtShare.Models;
using ShareItServices.UserService;

namespace ArtShare.Logic
{
    public class AccountLogic: IAccountLogic
    {

        private string token = "7dac496c534911c0ef47bce1de772502b0d6a6c60b1dbd73c1d3f285f36a0f61";


        public void RegisterAccount(RegisterModel model)
        {
            if (model.Password != model.RetypePassword)
            {
                throw new InvalidDataException("Retyped password does not match");
            }

            var user = new UserDTO()
            {
                Username = model.Username,
                Password = model.Password
            };
            
            using (var us = new UserServiceClient())
            {
                us.CreateAccount(user, token);
            }

        }


        public void DeleteAccount(int id)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAccountInformation(UpdateAccountModel model)
        {
            throw new System.NotImplementedException();
        }


        private UserDTO PassUserInformation(FormCollection collection)
        {

            return new UserDTO()
            {
                Username = collection["username"],
                Password = collection["password"]
            };
        }

        private UserDTO PassRequestingUserInformation(FormCollection collection)
        {

            return new UserDTO()
            {
                Username = collection["username"],
                Password = collection["password"]
            };
        }

        
    }
}