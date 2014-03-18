using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer.Stub
{
    class UserLogicStub : IUserLogic
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a UserLogic which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public UserLogicStub()
        {
            _factory = BusinessLogicFacade.GetTestFactory();
        }

        /// <summary>
        /// Construct a UserLogic object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the UserService should use for its logic.</param>
        public UserLogicStub(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        public bool CreateAccount(User user, string clientToken)
        {

            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            // Check if the user is already stored in the DB

            // Check constraints on username and password
            if (user.Username.Length < 1 || user.Username.Length > 20)
            {
                throw new ArgumentException("Username must consist of between 1 and 20 characters");
            } 
            else if (user.Password.Length < 1 || user.Password.Length > 50)
            {
                throw new ArgumentException("Password must consist of between 1 and 50 characters");
            }
            else if (!Regex.IsMatch(user.Username, "^[a-zA-Z0-9]+$*"))
            {
                throw new ArgumentException("Username must only consist of alphanumerical characters (a-zA-Z0-9)");
            }
            else if (Regex.IsMatch(user.Password, "\\s"))
            {
                throw new ArgumentException("Password must not contain any whitespace characters");
            }

            try
            {
                // Store the user account
            }
            catch (Exception)
            {
                throw new Exception("The account was not created");
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
                (!_factory.CreateAuthLogic().IsUserAdminOnClient(requestingUser, clientToken, new StorageBridge(new EfStorageConnection<BNDNEntities>()))))
            {
                throw new UnauthorizedAccessException();
            }
            
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
                (!_factory.CreateAuthLogic().IsUserAdminOnClient(requestingUser, clientToken, new StorageBridge(new EfStorageConnection<BNDNEntities>()))))
            {
                throw new UnauthorizedAccessException();
            }

            // Get the account from db

            User oldAccount;

            oldAccount = requestingUser;

            try
            {
                // Store the user account
            }
            catch (Exception)
            {
               throw new Exception("The account was not updated");
            }

            return true;
        }
    }
}
