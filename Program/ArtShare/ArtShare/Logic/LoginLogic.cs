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
        /// <summary>
        /// Logged a user into the system
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>A LoginModel containing the user and the users logged in state</returns>
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