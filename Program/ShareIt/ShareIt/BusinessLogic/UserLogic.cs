using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public class UserLogic : IUserLogic
    {

        private IBusinessLogicFactory _factory;
        private IStorageBridge _storage;

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
            else if (user.Password.Length < 1 || user.Password.Length > 50)
            {
                throw new ArgumentException("Password must consist of between 1 and 50 characters");
            }
            else if (Regex.IsMatch(user.Username, "^[a-zA-Z0-9]+$*"))
            {
                throw new ArgumentException("Username must only consist of alphanumerical characters (a-zA-Z0-9)");
            }
            else if (Regex.IsMatch(user.Password, "\\s"))
            {
                throw new ArgumentException("Password must not contain any whitespace characters");
            }

            // Attempt to create the user account
            try
            {
                var userAcc = new UserAcc()
                {
                    Username = user.Username,
                    Password = user.Password,
                    AcessRight = new Collection<AcessRight>(),
                    UserInfo = new Collection<UserInfo>(),
                    ClientAdmin = new Collection<ClientAdmin>()
                };
                _storage.Add<UserAcc>(userAcc);
            }
            catch (Exception e)
            {
                throw new Exception("The account could not be created");
            }

            return true;
        }

        public User GetAccountInformation(User requestingUser, User targetUser, string clientToken)
        {
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

            // get the account

            return targetUser;
        }

        public bool UpdateAccountInformation(User requestingUser, User newUser, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if ((!_factory.CreateAuthLogic().CheckUserExists(requestingUser) &&
                (requestingUser.Username != newUser.Username)) &&
                (!_factory.CreateAuthLogic().IsUserAdminOnClient(requestingUser, clientToken)))
            {
                throw new UnauthorizedAccessException();
            }

            // Get the old account from db
            
            // store the new account

            return true;
        }
    }
}
