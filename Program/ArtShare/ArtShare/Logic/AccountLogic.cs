using System;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using ArtShare.Models;
using ShareItServices.UserService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ArtShare.Logic
{
    public class AccountLogic: IAccountLogic
    {



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
                us.CreateAccount(user, Properties.Resources.ClientToken);
            }

        }


        public void DeleteAccount(string username, string password, int id)
        {
            if (username == null) { throw new ArgumentNullException("username"); }
            if (password == null) { throw new ArgumentNullException("password"); }

            using (var client = new UserServiceClient())
            {
                var admin = new UserDTO { Username = username, Password = password };
                client.DeleteAccount(admin, id, Properties.Resources.ClientToken);
            }
        }

        /// <summary>
        /// Update a user account
        /// </summary>
        /// <param name="username">Users current username</param>
        /// <param name="password">Users current password</param>
        /// <param name="model">The model to update the account from</param>
        public void UpdateAccountInformation(string username, string password, AccountModel model)
        {
            using (var us = new UserServiceClient())
            {
                var user = new UserDTO
                {
                    Id = model.Id,
                    Username = model.Username,
                    Password = model.Password,
                    Information = new UserInformationDTO[]{
                        new UserInformationDTO{
                            Type = UserInformationTypeDTO.Email,
                            Data = model.Email
                        },
                        new UserInformationDTO{
                            Type = UserInformationTypeDTO.Firstname,
                            Data = model.Firstname
                        },
                        new UserInformationDTO{
                            Type = UserInformationTypeDTO.Lastname,
                            Data = model.Lastname
                        },
                        new UserInformationDTO{
                            Type = UserInformationTypeDTO.Location,
                            Data = model.Location
                        }
                    }
                };

                us.UpdateAccounInformation(new UserDTO { Username = username, Password = password }, user, Properties.Resources.ClientToken);
            }
        }


        /// <summary>
        /// Fetching a users account information
        /// </summary>
        /// <param name="username">Fetching users username</param>
        /// <param name="password">Fetching users password</param>
        /// <param name="userId">The user who's account information to fetch</param>
        /// <returns>An accountmodel containg the users info</returns>
        public AccountModel GetAccountInformation(string username, string password, int userId)
        {
            using (var us = new UserServiceClient())
            {
                //Calling the client service
                var user = us.GetAccountInformation(new UserDTO { Username = username, Password = password }, userId, Properties.Resources.ClientToken);

                var accountModel = ExtractAccountInformation(user);

                #region Purchase history fetch
                List<AccountModel.PurchaseDTO> purchaseHistory;

                using (var ars = new ShareItServices.AccessRightService.AccessRightServiceClient()){
                    purchaseHistory = ars.GetPurchaseHistory(new ShareItServices.AccessRightService.UserDTO { Username = username, Password = password },
                    userId,
                    Properties.Resources.ClientToken).Select(t => new AccountModel.PurchaseDTO
                    {
                        MediaItemId = t.Id
                    }).ToList();

                    using (var mis = new ShareItServices.MediaItemService.MediaItemServiceClient())
                    {
                        foreach (var mi in purchaseHistory)
                        {
                            try
                            {
                                mi.Title = mis.GetMediaItemInformation
                                    (mi.MediaItemId,
                                    new ShareItServices.MediaItemService.UserDTO { Username = username, Password = password },
                                    Properties.Resources.ClientToken).Information.Single(t => t.Type == ShareItServices.MediaItemService.InformationTypeDTO.Title).Data;
                            }
                            catch{

                            }
                        }
                    }
                }
                #endregion

                //Returning accountmodel
                accountModel.PurchaseHistory = purchaseHistory;
                return accountModel;
            }
        }

        public UserListModel GetAllUsers(string username, string password)
        {
            if (username == null) { throw new ArgumentNullException("username"); }
            if (password == null) { throw new ArgumentNullException("password"); }

            using (var client = new UserServiceClient())
            {
                var admin = new UserDTO {Username = username, Password = password};
                var users = client.GetAllUsers(admin, Properties.Resources.ClientToken);
                
                var model = new UserListModel { Users = new List<AccountModel>() };
                foreach (var user in users)
                {
                    model.Users.Add(ExtractAccountInformation(user));
                }

                return model;
            }
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

        public AccountModel ExtractAccountInformation(UserDTO user)
        {
           if(user == null) { throw new ArgumentNullException("user"); }

             //Fetching information
            var email = user.Information != null ? user.Information.SingleOrDefault(t => t.Type == UserInformationTypeDTO.Email) : null;
            var firstname = user.Information != null ? user.Information.SingleOrDefault(t => t.Type == UserInformationTypeDTO.Firstname) : null;
            var lastname = user.Information != null ? user.Information.SingleOrDefault(t => t.Type == UserInformationTypeDTO.Lastname) : null;
            var location = user.Information != null ? user.Information.SingleOrDefault(t => t.Type == UserInformationTypeDTO.Location) : null;

            return new AccountModel
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Email = email != null ? email.Data : "",
                Firstname = firstname != null ? firstname.Data : "",
                Lastname = lastname != null ? lastname.Data : "",
                Location = location != null ? location.Data : ""
            };
        }
    }
}