using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Services;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareIt
{
    /// <summary>
    /// This service exposes validation of user and client credentials
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IBusinessLogicFactory _factory = BusinessLogicFacade.GetBusinessFactory();


        #region constructors

        /// <summary>
        /// Default constructor initialized by WCF
        /// </summary>
        public AuthService() { }


        /// <summary>
        /// Constructor with injection for testing
        /// </summary>
        /// <param name="factory">Business logic factory to use</param>
        public AuthService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        #endregion




        #region methods

        /// <summary>
        /// Validates whether a user exists with the system
        /// </summary>
        /// <param name="user">A user to check</param>
        /// <param name="clientToken">The token to varify the client asking</param>
        /// <returns>A boolean result</returns>
        public int ValidateUser(UserDTO user, string clientToken)
        {

            try
            {

                int result = -1;

                using (var al = _factory.CreateAuthLogic())
                {
                    result = al.CheckUserExists(user, clientToken);
                }

                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser(){Message = e.Message});
            }
            catch(ArgumentException e)
            {
                throw new FaultException<ArgumentFault>(new ArgumentFault() { Message = e.Message });
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }

        }


        /// <summary>
        /// Validates whether a client exists with the system
        /// </summary>
        /// <param name="client">A client to check</param>
        /// <returns>A boolean result</returns>
        public bool CheckClientExists(ClientDTO client)
        {
            try
            {
                bool result;

                using (var al = _factory.CreateAuthLogic())
                {
                    result = al.CheckClientExists(client);
                }

                return result;
            }
            catch (ArgumentException e)
            {
                throw new FaultException<ArgumentFault>(new ArgumentFault() { Message = e.Message });
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }

        }

        public bool IsUserAdminOnClient(UserDTO user, string clientToken)
        {
            try
            {
                bool result;

                using (var al = _factory.CreateAuthLogic())
                {
                    result = al.IsUserAdminOnClient(user, clientToken);
                }

                return result;
            }
            catch (ArgumentException e)
            {
                throw new FaultException<ArgumentFault>(new ArgumentFault() { Message = e.Message });
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        #endregion
    }
}
