using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text.RegularExpressions;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    internal class UserLogic : IUserLogic
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
                _storage.Add(userAcc);
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
            Contract.Requires<ArgumentNullException>(targetUserId != 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

          
            if (_authLogic.CheckClientToken(clientToken) == -1)
            {
                throw new InvalidCredentialException();
            }

            requestingUser.Id = _authLogic.CheckUserExists(requestingUser);

            if ((requestingUser.Id == -1 && (requestingUser.Id != targetUserId)) && (!_authLogic.IsUserAdminOnClient(requestingUser.Id, clientToken)))
            {
                throw new UnauthorizedAccessException();
            }

            try
            {
              var user = _storage.Get<UserAcc>().Single(t => t.Id == targetUserId);
              var targetUser = new UserDTO
              {
                  Id = user.Id,
                  Username = user.Username,
                  Password = user.Password,
                  Information = user.UserInfo.Select(t => new UserInformationDTO
                  {
                      Data = t.Data,
                      Type = (UserInformationTypeDTO)t.UserInfoType
                  })
              };

                

              return targetUser;
            }
            catch (Exception e)
            {
                throw new Exception("The requested user could not be found");
            }
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
                if (userToUpdate.Id == 0)
                {
                    currentUserAcc =
                        (from u in _storage.Get<UserAcc>() where u.Username == userToUpdate.Username select u).First();
                }
                else
                {
                    currentUserAcc = (from u in _storage.Get<UserAcc>() where u.Id == userToUpdate.Id select u).First();
                        //TODO should probably not be id
                }
            }
            catch (Exception)
            {
                throw new Exception("User to be updated was not found in the database");
            }

            // Attempt to update the user account by inserting it with the same id
            //try
            //{
            ValidatePassword(userToUpdate);
            currentUserAcc.Password = userToUpdate.Password;

            _storage.Delete(currentUserAcc.UserInfo);

            currentUserAcc.UserInfo = userToUpdate.Information.Select(x => new UserInfo
            {
                Data = x.Data,
                UserInfoType = (int) x.Type
            }).ToList();
 
            _storage.Update(currentUserAcc);
 
            return true;
        }

        /// <summary>
        /// Get a list of all users.
        /// </summary>
        /// <param name="admin">The admin requesting the list</param>
        /// <param name="clientToken">The client from which the request originated.</param>
        /// <returns>A list of all users including their id and username, but not their password.</returns>
        public IList<UserDTO> GetAllUsers(UserDTO admin, string clientToken)
        {
            Contract.Requires<ArgumentNullException>(admin != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(admin.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(admin.Password));
            Contract.Requires<ArgumentNullException>(clientToken != null);


            if (_authLogic.CheckClientToken(clientToken) < 1)
            {
                throw new InvalidClientException();
            }
            if (_authLogic.CheckUserExists(admin) == -1)
            {
                throw new InvalidUserException();
            }
            if (!_authLogic.IsUserAdminOnClient(admin, clientToken))
            {
                throw new UnauthorizedUserException();
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
        /// <param name="clientToken">The client from which the request originated.</param>
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
                var msg = "Client token not valid.";
                throw new FaultException<UnauthorizedClient>(new UnauthorizedClient()
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            var userId = _authLogic.CheckUserExists(requestingUser);
            if (userId == -1)
            {
                var msg = "User credentials not valid.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            if (!_authLogic.IsUserAdminOnClient(userId, clientToken) && userId != userToBeDeletedId)
            {
                var msg = "User not allowed to delete user with id: " + userToBeDeletedId + ",\n" +
                              "because he is not admin and tying to delete another user than himself.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = msg
                }, new FaultReason(msg));
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
