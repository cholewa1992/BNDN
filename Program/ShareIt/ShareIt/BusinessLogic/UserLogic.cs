using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text.RegularExpressions;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public class UserLogic : IUserLogic
    {

        private readonly IAuthInternalLogic _authLogic;
        private readonly IStorageBridge _storage;

        /// <summary>
        /// Construct a UserLogic which is used by the default business logic factory.
        /// </summary>
        internal UserLogic(IStorageBridge storage, IAuthInternalLogic authLogic)
        {
            _storage = storage;
            _authLogic = authLogic;
        }

        /// <summary>
        /// Creates a user account.
        /// </summary>
        /// <param name="user">The user to be created</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns>Boolean value depending on the result</returns>
        public bool CreateAccount(UserDTO user, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            // Check if the clientToken is valid
            if (_authLogic.CheckClientToken(clientToken) == -1)
            {
                throw new InvalidCredentialException();
            }

            // Check if the user is already stored in the DB
            if (_storage.Get<UserAcc>().Any(x => x.Username == user.Username))
            {
                throw new Exception("Username already in use");
            }

            // Check constraints on username
            if (user.Username.Length < 1 || user.Username.Length > 20)
            {
                throw new ArgumentException("Username must consist of between 1 and 20 characters");
            }
            if (Regex.IsMatch(user.Username, "[^a-zA-Z0-9]"))
            {
                throw new ArgumentException("Username must only consist of alphanumerical characters (a-zA-Z0-9)");
            }
            //Check constraints on password
            ValidatePassword(user);

            // Attempt to create the user account
            try
            {
                var userAcc = new UserAcc
                {
                    Username = user.Username,
                    Password = user.Password,
                    AccessRight = new Collection<AccessRight>(),
                    UserInfo = new Collection<UserInfo>(),
                    ClientAdmin = new Collection<ClientAdmin>()
                };
                _storage.Add<UserAcc>(userAcc);
            }
            catch (Exception)
            {
                throw new Exception("The account could not be created");
            }

            return true;
        }

        public UserDTO GetAccountInformation(UserDTO requestingUser, int targetUserId, string clientToken)
        {

            //Preconditions
            Contract.Requires<ArgumentNullException>(requestingUser != null);
            Contract.Requires<ArgumentNullException>(requestingUser.Id != 0);
            Contract.Requires<ArgumentNullException>(targetUserId != 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            if (_authLogic.CheckClientToken(clientToken) == -1)
            {
                throw new InvalidCredentialException();
            }

            if ((_authLogic.CheckUserExists(requestingUser) == -1 &&
                (requestingUser.Id != targetUserId)) &&
                (!_authLogic.IsUserAdminOnClient(requestingUser.Id, clientToken)))
            {
                throw new UnauthorizedAccessException();
            }
            
            IEnumerable<UserInfo> userInfos;

            try
            {
                // Get the userinformation belonging to the user with the requested ID
                userInfos = (from u in _storage.Get<UserInfo>() where u.UserId == targetUserId select u);
            }
            catch (Exception e)
            {
                throw new Exception("The requested user could not be found");
            }

            // Create a UserInformation in the user we want to return
            var targetUser = new UserDTO {Information = new List<UserInformationDTO>()};
            var informationList = new List<UserInformationDTO>();

            // Add UserInformation to the temporary list object
            foreach (var userInfo in userInfos){

                informationList.Add(new UserInformationDTO()
                    {
                        Type = (UserInformationTypeDTO) userInfo.UserInfoType,
                        Data = userInfo.Data
                    }
                );
            }

            // Add all the UserInformation to targetUser and return it
            targetUser.Information = informationList;

            return targetUser;
        }

        public bool UpdateAccountInformation(UserDTO requestingUser, UserDTO userToUpdate, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(requestingUser != null);
            Contract.Requires<ArgumentNullException>(userToUpdate != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(userToUpdate.Password));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(userToUpdate.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(requestingUser.Password));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(requestingUser.Username));

            if (_authLogic.CheckClientToken(clientToken) == -1)
            {
                throw new InvalidCredentialException();
            }

            if ((_authLogic.CheckUserExists(requestingUser) == -1 &&
                (requestingUser.Username != userToUpdate.Username)) &&
                (!_authLogic.IsUserAdminOnClient(requestingUser.Id, clientToken)))
            {
                throw new UnauthorizedAccessException();
            }

            UserAcc currentUserAcc;

            try
            {
                 currentUserAcc = (from u in _storage.Get<UserAcc>() where u.Id == userToUpdate.Id select u).First(); //TODO should probably not be id
            }
            catch (Exception)
            {
                throw new Exception("User to be updated was not found in the database");
            }

            // Attempt to update the user account by inserting it with the same id
            try
            {
                ValidatePassword(userToUpdate);
                currentUserAcc.Password = userToUpdate.Password;
                currentUserAcc.UserInfo = userToUpdate.Information.Select(x => new UserInfo
                {
                    Data = x.Data,
                    UserInfoType = (int) x.Type
                }).ToList();
                _storage.Update(currentUserAcc);
                //var userAcc = new UserAcc()
                //{
                //    Id = userToUpdate.Id,
                //    Username = userToUpdate.Username,
                //    Password = userToUpdate.Password,
                //    AccessRight = currentUserAcc.AccessRight,
                //    UserInfo = currentUserAcc.UserInfo,
                //    ClientAdmin = currentUserAcc.ClientAdmin
                //};
                //_storage.Add<UserAcc>(userAcc);
            }
            catch (Exception e)
            {
                throw new Exception("The account could not be created");
            }

            return true;
        }
        /// <summary>
        /// Get a list of all users.
        /// </summary>
        /// <param name="admin">The admin requesting the list</param>
        /// <param name="clientToken">The client from which the request originated.</param>
        /// <returns>A list of all users including their id and username, but not their password.</returns>
        /// <exception cref="FaultException{UnauthorizedClient}">If the clientToken is not valid.</exception>
        /// <exception cref="FaultException{UnauthorizedUser}">If "admin" isn't an admin on the specified client.</exception>
        public IList<UserDTO> GetAllUsers(UserDTO admin, string clientToken)
        {
            Contract.Requires<ArgumentNullException>(admin != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(admin.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(admin.Password));
            Contract.Requires<ArgumentNullException>(clientToken != null);


            if (_authLogic.CheckClientToken(clientToken) < 1)
            {
                throw new FaultException<UnauthorizedClient>(new UnauthorizedClient()
                {
                    Message = "Client token not valid."
                });
            }
            if (!_authLogic.IsUserAdminOnClient(admin, clientToken))
            {
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = "Only admins can get a list of all users."
                });
            }

            return _storage.Get<UserAcc>().Select(x => new UserDTO()
            {
                Id = x.Id,
                Username = x.Username
            }).ToList();
        }
        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="requestingUser">The user who wishes to delete a user. Should be an admin or the same user as is being deleted.</param>
        /// <param name="userToBeDeletedId">The id of the user which is to be deleted.</param>
        /// <param name="clientToken">The client from which the request originatd.</param>
        /// <returns>True if the user was deleted, otherwise false.</returns>
        /// <exception cref="FaultException{UnauthorizedClient}">If the clientToken is not valid.</exception>
        /// <exception cref="FaultException{UnauthorizedUser}">If the requesting user isn't admin and trying to delete a user other than himself.</exception>
        public bool DeleteUser(UserDTO requestingUser, int userToBeDeletedId, string clientToken)
        {
            Contract.Requires<ArgumentNullException>(requestingUser != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);
            Contract.Requires<ArgumentException>(userToBeDeletedId > 0);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(requestingUser.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(requestingUser.Password));
            if (_authLogic.CheckClientToken(clientToken) < 1)
            {
                throw new FaultException<UnauthorizedClient>(new UnauthorizedClient()
                {
                    Message = "Client token not valid."
                });
            }
            var userId = _authLogic.CheckUserExists(requestingUser);
            if (!_authLogic.IsUserAdminOnClient(userId, clientToken) && userId != userToBeDeletedId)
            {
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = "User not allowed to delete user with id: " + userToBeDeletedId + ",\n" +
                              "because he is not admin and tying to delete another user than himself."
                });
            }
            _storage.Delete<UserAcc>(userToBeDeletedId);
            return true;
        }

        /// <summary>
        /// Validate that the password of a UserDTO lives up to requirements.
        /// Between 1 and 50 characters.
        /// Must not contain any whitespace characters.
        /// </summary>
        /// <param name="user">The user whose password should be validated.</param>
        private void ValidatePassword(UserDTO user)
        {
            if (user.Password.Length < 1 || user.Password.Length > 50)
            {
                throw new ArgumentException("Password must consist of between 1 and 50 characters");
            }
            if (Regex.IsMatch(user.Password, "\\s"))
            {
                throw new ArgumentException("Password must not contain any whitespace characters");
            }
        }

        public void Dispose()
        {
            if(_authLogic != null)
                _authLogic.Dispose();
            if(_storage != null)
                _storage.Dispose();
        }
    }
}
