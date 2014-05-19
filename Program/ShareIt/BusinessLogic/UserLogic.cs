using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text.RegularExpressions;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This class handles all User related operations
    /// </summary>
    /// <author>Nicki Jørgensen (nhjo@itu.dk)</author>
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
                throw new InvalidClientException();
            }

            // Check if the user is already stored in the DB
            if (_storage.Get<UserAcc>().Any(x => x.Username.ToLower() == user.Username.ToLower()))
            {
                throw new ArgumentException("Username already in use");
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

            // Create the user account
            var userAcc = new UserAcc
            {
                Username = user.Username,
                Password = user.Password,
                AccessRight = new Collection<AccessRight>(),
                UserInfo = new Collection<UserInfo>(),
                ClientAdmin = new Collection<ClientAdmin>()
            };
            _storage.Add(userAcc);

            return true;
        }

        /// <summary>
        /// This method gets all the account information an account contains and returns it in a UserDTO
        /// </summary>
        /// <param name="requestingUser">The user requesting the operation</param>
        /// <param name="targetUserId">The id of the users whose information should be returned</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>A UserDTO containing all the account information related to the target user</returns>
        public UserDTO GetAccountInformation(UserDTO requestingUser, int targetUserId, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(requestingUser != null);
            Contract.Requires<ArgumentNullException>(targetUserId != 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

          
            if (_authLogic.CheckClientToken(clientToken) == -1)
            {
                throw new InvalidClientException();
            }

            if(!string.IsNullOrEmpty(requestingUser.Username) && !string.IsNullOrEmpty(requestingUser.Password))
                requestingUser.Id = _authLogic.CheckUserExists(requestingUser);
                
            bool sendPassword =  requestingUser.Id == targetUserId || ( requestingUser.Id > 0 && _authLogic.IsUserAdminOnClient(requestingUser.Id, clientToken));


            var user = _storage.Get<UserAcc>().SingleOrDefault(t => t.Id == targetUserId);

            if (user == null)
            {
                throw new UserNotFoundException("The target user could not be found.");
            }
                
            var targetUser = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Password = sendPassword ? user.Password : null,
                Information = user.UserInfo.Select(t => new UserInformationDTO
                {
                    Data = t.Data,
                    Type = (UserInformationTypeDTO)t.UserInfoType
                })
            };

            return targetUser;
        }

        /// <summary>
        /// This method updates the account information of an account. 
        /// It overrides all the information that the account currently contains with the new information it is given
        ///  via the userToUpdate contains.
        /// </summary>
        /// <param name="requestingUser">The user requesting the operation</param>
        /// <param name="userToUpdate">A UserDTO containing all the information the account will have after the update</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the operation is successful, otherwise an exception is thrown</returns>
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
                throw new InvalidClientException();
            }

            if (_authLogic.CheckUserExists(requestingUser) == -1)
            {
                throw new InvalidUserException();
            }

            requestingUser.Id = _authLogic.CheckUserExists(requestingUser);

            Console.WriteLine(requestingUser.Id);

            if (((requestingUser.Username != userToUpdate.Username) &&
                (!_authLogic.IsUserAdminOnClient(requestingUser.Id, clientToken))))
            {
                throw new UnauthorizedUserException();
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
                }
            }
            catch (InvalidOperationException e)
            {
                throw new UserNotFoundException("The target user could not be found.");
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
                throw new InvalidClientException();
            }
            var userId = _authLogic.CheckUserExists(requestingUser);
            if (userId == -1)
            {
                throw new InvalidUserException();
            }
            if (!_authLogic.IsUserAdminOnClient(userId, clientToken) && userId != userToBeDeletedId)
            {
                throw new UnauthorizedUserException();
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
