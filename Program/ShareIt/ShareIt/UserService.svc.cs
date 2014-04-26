using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UserService.svc or UserService.svc.cs at the Solution Explorer and start debugging.
    public class UserService : IUserService
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a UserService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public UserService()
        {
            _factory = BusinessLogicEntryFactory.GetBusinessFactory();
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
        public bool CreateAccount(UserDTO user, string clientToken)
        {
            try
            {
                return _factory.CreateUserLogic().CreateAccount(user, clientToken);
            }
            catch (InvalidCredentialException)
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(fault.Message));
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault();
                fault.Message = ae.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
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
        /// <param name="targetUserId">The id of the user of which you want to fetch account information</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns></returns>
        public UserDTO GetAccountInformation(UserDTO requestingUser, int targetUserId, string clientToken)
        {
            try
            {
                return _factory.CreateUserLogic().GetAccountInformation(requestingUser, targetUserId, clientToken);
            }
            catch (InvalidCredentialException)
            {
                var fault = new UnauthorizedClient();
                var msg = "The Client is not authorized to perform this request.";
                fault.Message = msg;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(msg));
            }
            catch (UnauthorizedAccessException)
            {
                var fault = new UnauthorizedUser();
                var msg = "The User is not authorized to perform this request.";
                fault.Message = msg;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(msg));
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault();
                fault.Message = ae.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
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
        public bool UpdateAccounInformation(UserDTO requestingUser, UserDTO newUser, string clientToken)
        {
            try
            {
                return _factory.CreateUserLogic().UpdateAccountInformation(requestingUser, newUser, clientToken);
            }
            catch (InvalidCredentialException)
            {
                var fault = new UnauthorizedClient();
                var msg = "The Client is not authorized to perform this request.";
                fault.Message = msg;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(msg));
            }
            catch (UnauthorizedAccessException)
            {
                var fault = new UnauthorizedUser();
                var msg = "The User is not authorized to perform this request.";
                fault.Message = msg;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(msg));
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault();
                fault.Message = ae.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        public IList<UserDTO> GetAllUsers(UserDTO admin, string clientToken)
        {
            try
            {
                IList<UserDTO> result;
                using (var logic = _factory.CreateUserLogic())
                {
                    result = logic.GetAllUsers(admin, clientToken);
                }
                return result;
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault();
                fault.Message = ae.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
            }
            catch (InvalidClientException)
            {
                var msg = "Client token not valid.";
                var fault = new UnauthorizedClient()
                {
                    Message = msg
                };

                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(msg));
            }
            catch (InvalidUserException)
            {
                var msg = "User credentials not valid.";
                var fault = new UnauthorizedUser()
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(msg));
            }
            catch (UnauthorizedUserException)
            {
                var msg = "Only admins can get a list of all users.";
                var fault = new UnauthorizedUser()
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(msg));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        public bool DeleteAccount(UserDTO requestingUser, int acountToBeDeletedId, string clientToken)
        {
            try
            {
                var result = false;
                using (var logic = _factory.CreateUserLogic())
                {
                    result = logic.DeleteUser(requestingUser, acountToBeDeletedId, clientToken);
                }
                return result;
            }
            catch (InvalidClientException e)
            {
                var msg = "Client token not valid.";
                throw new FaultException<UnauthorizedClient>(new UnauthorizedClient()
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (InvalidUserException e)
            {
                var msg = "User credentials not valid.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (UnauthorizedUserException e)
            {
                var msg = "User not allowed to delete user with id: " + acountToBeDeletedId + ",\n" +
                              "because he is not admin and trying to delete another user than himself.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault();
                fault.Message = ae.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
