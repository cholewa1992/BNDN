using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in both code and config file together.
    public class UserService : IUserService
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a UserService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public UserService()
        {
            _factory = BusinessLogicFacade.GetBusinessFactory();
        }

        /// <summary>
        /// Construct a UserService object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the UserService should use for its logic.</param>
        public UserService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Creates an account
        /// </summary>
        /// <param name="user">The user to be created</param>
        /// <param name="clientToken">Token used to validate the client</param>
        public bool CreateAccount(User user, string clientToken)
        {
            try
            {
                return _factory.CreateUserLogic().CreateAccount(user, clientToken);
            }
            catch (InvalidCredentialException)
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (ArgumentException)
            {
                return true;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Returns account information
        /// </summary>
        /// <param name="requestingUser">The user performing the request</param>
        /// <param name="targetUser">The user of which you want to fetch account information</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns></returns>
        public User GetAccountInformation(User requestingUser, User targetUser, string clientToken)
        {
            try
            {
                return _factory.CreateUserLogic().GetAccountInformation(requestingUser, targetUser, clientToken);
            }
            catch (InvalidCredentialException)
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException)
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Update a user account
        /// </summary>
        /// <param name="newUser">The user to be updated</param>
        /// <param name="requestingUser">The user performing the request</param>
        /// <param name="clientToken">Token used to validate the client</param>
        public bool UpdateAccounInformation(User requestingUser, User newUser, string clientToken)
        {
            try
            {
                return _factory.CreateUserLogic().UpdateAccountInformation(requestingUser, newUser, clientToken);
            }
            catch (InvalidCredentialException)
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException)
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
