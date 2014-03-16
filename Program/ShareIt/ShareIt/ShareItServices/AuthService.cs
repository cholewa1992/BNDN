using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AuthService" in both code and config file together.
    public class AuthService : IAuthService
    {

        private readonly IBusinessLogicFactory _factory = BusinessLogicFacade.GetBusinessFactory();


#region constructors

        /// <summary>
        /// Default constructor initialized by WCF
        /// </summary>
        public AuthService() { }


        /// <summary>
        /// Constructor with injection
        /// </summary>
        /// <param name="factory">Business logic factory to use</param>
        public AuthService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

#endregion

        


#region methods

        /// <summary>
        /// Validates whether a user exists in the system
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool ValidateUser(User user)
        {
            
            try
            {
                return _factory.CreateAuthLogic().CheckUserExists(user);
            }
            catch (Exception)
            {
                throw new FaultException();
            }

        }

        /// <summary>
        /// Validates whether a client exists in the system
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckClientPassword(Client client)
        {
            try
            {
                return _factory.CreateAuthLogic().CheckClientPassword(client);
            }
            catch (Exception)
            {
                throw new FaultException();
            }

        }

#endregion

        
    }
}
