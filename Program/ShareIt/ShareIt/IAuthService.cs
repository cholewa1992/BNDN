using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Web.Services;
using System.Web.Services.Description;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAuthService" in both code and config file together.
    [ServiceContract]
    public interface IAuthService
    {

        /// <summary>
        /// Validates whether a user exists with the system
        /// </summary>
        /// <param name="user">A user to check</param>
        /// <param name="clientToken">The token to varify the client asking</param>
        /// <returns>A boolean result</returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        int ValidateUser(UserDTO user, string clientToken);

        /// <summary>
        /// Validates whether a client exists with the system
        /// </summary>
        /// <param name="client">A client to check</param>
        /// <returns>A boolean result</returns>
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool CheckClientExists(ClientDTO client);

        /// <summary>
        /// Validates whether a given user is an administrator on a given client
        /// </summary>
        /// <param name="user">given user</param>
        /// <param name="clientToken">given client</param>
        /// <returns>a bool answer</returns>
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool IsUserAdminOnClient(UserDTO user, string clientToken);
    }
}
