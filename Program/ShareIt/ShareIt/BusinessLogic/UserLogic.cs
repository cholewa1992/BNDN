using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public class UserLogic : IUserLogic
    {

        private readonly IAuthInternalLogic _authLogic;
        private readonly IBusinessLogicFactory _factory;
        private readonly IStorageBridge _storage;

        /// <summary>
        /// Construct a UserLogic which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        internal UserLogic(IStorageBridge storage, IAuthInternalLogic authLogic)
        {
            _storage = storage;
            _factory = BusinessLogicFacade.GetTestFactory();
            //_factory = BusinessLogicFacade.GetBusinessFactory();
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

            // Check constraints on username and password
            if (user.Username.Length < 1 || user.Username.Length > 20)
            {
                throw new ArgumentException("Username must consist of between 1 and 20 characters");
            }
            if (user.Password.Length < 1 || user.Password.Length > 50)
            {
                throw new ArgumentException("Password must consist of between 1 and 50 characters");
            }
            if (Regex.IsMatch(user.Username, "[^a-zA-Z0-9]"))
            {
                throw new ArgumentException("Username must only consist of alphanumerical characters (a-zA-Z0-9)");
            }
            if (Regex.IsMatch(user.Password, "\\s"))
            {
                throw new ArgumentException("Password must not contain any whitespace characters");
            }

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
                 currentUserAcc = (from u in _storage.Get<UserAcc>() where u.Id == userToUpdate.Id select u).First();
            }
            catch (Exception)
            {
                throw new Exception("User to be updated was not found in the database");
            }

            // Attempt to update the user account by inserting it with the same id
            try
            {
                var userAcc = new UserAcc()
                {
                    Id = userToUpdate.Id,
                    Username = userToUpdate.Username,
                    Password = userToUpdate.Password,
                    AccessRight = currentUserAcc.AccessRight,
                    UserInfo = currentUserAcc.UserInfo,
                    ClientAdmin = currentUserAcc.ClientAdmin
                };
                _storage.Add<UserAcc>(userAcc);
            }
            catch (Exception e)
            {
                throw new Exception("The account could not be created");
            }

            return true;
        }
    }
}
