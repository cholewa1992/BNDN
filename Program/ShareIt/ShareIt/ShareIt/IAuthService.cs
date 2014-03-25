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
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool ValidateUser(User user, string clientToken);

        /// <summary>
        /// Validates whether a client exists with the system
        /// </summary>
        /// <param name="client">A client to check</param>
        /// <returns>A boolean result</returns>
        [OperationContract]
        bool CheckClientExists(Client client);
    }
}
