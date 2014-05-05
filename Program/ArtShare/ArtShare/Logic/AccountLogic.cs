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
    /// <summary>
    /// Logic for account operation
    /// </summary>
    /// <author>
    /// Jacob B. Cholewa (jbec@itu.dk)
    /// </author>
    public class AccountLogic: IAccountLogic
    {
        /// <summary>
        /// Register a new account
        /// </summary>
        /// <param name="model">The model containing the account to register</param>
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

        /// <summary>
        /// Delete account
        /// </summary>
        /// <param name="username">The username of invoking user</param>
        /// <param name="password">The username of invoking user</param>
        /// <param name="id">Id of the user to delete</param>
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
        /// Checks weather or not a user has access right to edit accountinfo of target user
        /// </summary>
        /// <param name="username">The username of invoking user</param>
        /// <param name="password">The username of invoking user</param>
        /// <param name="userId">Id of the target user</param>
        /// <returns>True if invoking user has the right to edit target user</returns>
        public bool HasRightToEdit(string username, string password, int userId)
        {
            using(var us = new UserServiceClient()){
                if(userId == new LoginLogic().Login(username, password).User.Id) return true;

                using(var ars = new ShareItServices.AuthService.AuthServiceClient()){
                    return ars.IsUserAdminOnClient(new ShareItServices.AuthService.UserDTO{Username = username, Password = password}, Properties.Resources.ClientToken);
                }
            }
        }

        /// <summary>
        /// Update a user account
        /// </summary>
        /// <param name="username">The username of invoking user</param>
        /// <param name="password">The username of invoking user</param>
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
        /// <param name="username">The username of invoking user</param>
        /// <param name="password">The username of invoking user</param>
        /// <param name="userId">The user who's account information to fetch</param>
        /// <returns>An accountmodel containg the users info</returns>
        public AccountModel GetAccountInformation(string username, string password, int userId)
        {
            using (var us = new UserServiceClient())
            {
                //Calling the client service
                var user = us.GetAccountInformation(new UserDTO { Username = username, Password = password }, userId, Properties.Resources.ClientToken);

                var accountModel = ExtractAccountInformation(user);

                #region Upload history fetch
                try
                {
                    List<AccountModel.PurchaseDTO> uploadHistory;
                    using (var ars = new ShareItServices.AccessRightService.AccessRightServiceClient())
                    {
                        uploadHistory = ars.GetUploadHistory(
                            null,
                            userId,
                            Properties.Resources.ClientToken
                            ).Select(t => new AccountModel.PurchaseDTO
                            {
                                MediaItemId = t.MediaItemId
                            }).ToList();

                        using (var mis = new ShareItServices.MediaItemService.MediaItemServiceClient())
                        {
                            foreach (var mi in uploadHistory)
                            {
                                try
                                {
                                    var dto = mis.GetMediaItemInformation(
                                        mi.MediaItemId,
                                        null,
                                        Properties.Resources.ClientToken);

                                    mi.Title = dto.Information.Single(t => t.Type == ShareItServices.MediaItemService.InformationTypeDTO.Title).Data;
                                    mi.Thumbnail = dto.Information.Single(t => t.Type == ShareItServices.MediaItemService.InformationTypeDTO.Thumbnail).Data;
                                }
                                catch
                                {

                                }
                            }
                        }
                    }

                    accountModel.UploadHistory = uploadHistory;
                }
                catch
                {

                }
                #endregion
                #region Purchase history fetch
                try
                {
                    List<AccountModel.PurchaseDTO> purchaseHistory;

                    using (var ars = new ShareItServices.AccessRightService.AccessRightServiceClient())
                    {
                        purchaseHistory = ars.GetPurchaseHistory(new ShareItServices.AccessRightService.UserDTO { Username = username, Password = password },
                        userId,
                        Properties.Resources.ClientToken).Select(t => new AccountModel.PurchaseDTO
                        {
                            MediaItemId = t.MediaItemId
                        }).ToList();

                        using (var mis = new ShareItServices.MediaItemService.MediaItemServiceClient())
                        {
                            foreach (var mi in purchaseHistory)
                            {
                                try
                                {
                                    var dto = mis.GetMediaItemInformation(
                                        mi.MediaItemId,
                                        new ShareItServices.MediaItemService.UserDTO { Username = username, Password = password },
                                        Properties.Resources.ClientToken);

                                    mi.Title = dto.Information.Single(t => t.Type == ShareItServices.MediaItemService.InformationTypeDTO.Title).Data;
                                    mi.Thumbnail = dto.Information.Single(t => t.Type == ShareItServices.MediaItemService.InformationTypeDTO.Thumbnail).Data;
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    //Returning accountmodel
                    accountModel.PurchaseHistory = purchaseHistory;
                }
                catch
                {
                }
                #endregion


                using(var asc = new ShareItServices.AuthService.AuthServiceClient()){
                 accountModel.CanEdit = 
                     (accountModel.Username == username && accountModel.Password == password) || 
                     asc.IsUserAdminOnClient(
                        new ShareItServices.AuthService.UserDTO { 
                            Username = username, 
                            Password = password 
                        }, Properties.Resources.ClientToken );
                }

                return accountModel;
            }
        }

        /// <summary>
        /// Gets a list of all users
        /// </summary>
        /// <param name="username">The username of invoking user</param>
        /// <param name="password">The username of invoking user</param>
        /// <returns>A list of all users on the client</returns>
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

        public bool IsUserAdmin(string username, string password, int userId)
        {
            using (var asc = new ShareItServices.AuthService.AuthServiceClient())
            {
                    return asc.IsUserAdminOnClient(
                       new ShareItServices.AuthService.UserDTO
                       {
                           Id = userId,
                           Username = username,
                           Password = password
                       }, Properties.Resources.ClientToken);
            }
        }
    }
}