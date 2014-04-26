using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUserService" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {
        /// <summary>
        /// Creates an account
        /// </summary>
        /// <param name="user">The user to be created</param>
        /// <param name="clientToken"></param>
        [OperationContract]
        bool CreateAccount(User user, string clientToken);

        /// <summary>
        /// Returns account information
        /// </summary>
        /// <param name="requestingUser">The user performing the request</param>
        /// <param name="targetUser">The user of which you want to fetch account information</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns></returns>
        [OperationContract]
        User GetAccountInformation(User requestingUser, User targetUser, string clientToken);

        /// <summary>
        /// Update a user account
        /// </summary>
        /// <param name="newUser">The user to be updated</param>
        /// <param name="requestingUser">The user performing the request</param>
        /// <param name="clientToken">Token used to validate the client</param>
        [OperationContract]
        bool UpdateAccounInformation(User requestingUser, User newUser, string clientToken);
    }
}
