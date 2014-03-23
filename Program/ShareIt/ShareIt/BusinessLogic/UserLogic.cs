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

        private readonly IBusinessLogicFactory _factory;
        private readonly IStorageBridge _storage;

        /// <summary>
        /// Construct a UserLogic which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public UserLogic(IStorageBridge storage)
        {
            _storage = storage;
            _factory = BusinessLogicFacade.GetTestFactory();
            //_factory = BusinessLogicFacade.GetBusinessFactory();
        }

        /// <summary>
        /// Creates a user account.
        /// </summary>
        /// <param name="user">The user to be created</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns>Boolean value depending on the result</returns>
        public bool CreateAccount(User user, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            // Check if the clientToken is valid
            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
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
            if (Regex.IsMatch(user.Username, "^[a-zA-Z0-9]+$*"))
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
                    AcessRight = new Collection<AcessRight>(),
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

        public User GetAccountInformation(User requestingUser, User targetUser, string clientToken)
        {

            //Preconditions
            Contract.Requires<ArgumentNullException>(requestingUser != null);
            Contract.Requires<ArgumentNullException>(targetUser != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if ((!_factory.CreateAuthLogic().CheckUserExists(requestingUser) &&
                (requestingUser.Username != targetUser.Username)) &&
                (!_factory.CreateAuthLogic().IsUserAdminOnClient(requestingUser, clientToken)))
            {
                throw new UnauthorizedAccessException();
            }

            if (!_factory.CreateAuthLogic().CheckUserExists(targetUser))
            {
                throw new Exception("User does not exist");
            }

            IEnumerable<UserInfo> userInfos;

            try
            {
                // Get the userinformation belonging to the user with the requested ID
                userInfos = (from u in _storage.Get<UserInfo>() where u.UserId == targetUser.Id select u);
            }
            catch (Exception e)
            {
                throw new Exception("The requested user could not be found");
            }

            // Create a UserInformation in the user we want to return
            targetUser = new User {Information = new List<UserInformation>()};
            var informationList = new List<UserInformation>();

            // Add UserInformation to the temporary list object
            foreach (var userInfo in userInfos){

                informationList.Add(new UserInformation()
                    {
                        Type = (UserInformationType) userInfo.UserInfoType,
                        Data = userInfo.Data
                    }
                );
            }

            // Add all the UserInformation to targetUser and return it
            targetUser.Information = informationList;

            return targetUser;
        }

        public bool UpdateAccountInformation(User requestingUser, User userToUpdate, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(requestingUser != null);
            Contract.Requires<ArgumentNullException>(userToUpdate != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if ((!_factory.CreateAuthLogic().CheckUserExists(requestingUser) &&
                (requestingUser.Username != userToUpdate.Username)) &&
                (!_factory.CreateAuthLogic().IsUserAdminOnClient(requestingUser, clientToken)))
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
                    AcessRight = currentUserAcc.AcessRight,
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
