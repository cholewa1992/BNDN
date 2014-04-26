using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtShare.Models;
using ShareItServices.AuthService;

namespace ArtShare.Logic
{
    public class LoginLogic
    {
        public LoginModel Login(string username, string password)
        {
            //Creates a new model
            var model = new LoginModel();

            //Opens a AuthServiceClient
            using (var client = new AuthServiceClient())
            {
                //Creating user object
                model.User = new UserDTO { Username = username, Password = password };
                //Validating user
                model.User.Id = client.ValidateUser(model.User, Properties.Resources.ClientToken);
                //If the user was validated, set LoggedIn property to true
                model.LoggedIn = model.User.Id > 0;
            }
            return model;
        }
    }
}