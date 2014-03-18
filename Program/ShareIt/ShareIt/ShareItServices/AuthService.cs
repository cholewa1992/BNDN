using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareItServices
{
    /// <summary>
    /// This service exposes validation of user and client credentials
    /// </summary>
    public class AuthService : IAuthService
    {

        private readonly IBusinessLogicFactory _factory = BusinessLogicFacade.GetTestFactory();


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
        /// <returns>A boolean result</returns>
        public bool ValidateUser(User user)
        {

            try
            {
                return _factory.CreateAuthLogic().CheckUserExists(user);
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
        public bool CheckClientPassword(Client client)
        {
            try
            {
                return _factory.CreateAuthLogic().CheckClientPassword(client);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }

        }

        #endregion


    }
}
