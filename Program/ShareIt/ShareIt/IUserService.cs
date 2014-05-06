using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareIt
{
    /// <author>
    /// Nicki Jørgensen (nhjo@itu.dk)
    /// </author>
    [ServiceContract]
    public interface IUserService
    {
        /// <summary>
        /// Creates an account
        /// </summary>
        /// <param name="user">The user to be created</param>
        /// <param name="clientToken"></param>
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool CreateAccount(UserDTO user, string clientToken);

        /// <summary>
        /// Returns account information
        /// </summary>
        /// <param name="requestingUser">The user performing the request</param>
        /// <param name="targetUserId">The id of the user of which you want to fetch account information</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns></returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        UserDTO GetAccountInformation(UserDTO requestingUser, int targetUserId, string clientToken);

        /// <summary>
        /// Update a user account
        /// </summary>
        /// <param name="newUser">The user to be updated</param>
        /// <param name="requestingUser">The user performing the request</param>
        /// <param name="clientToken">Token used to validate the client</param>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool UpdateAccounInformation(UserDTO requestingUser, UserDTO newUser, string clientToken);
        /// <summary>
        /// Get a list of all users.
        /// </summary>
        /// <param name="admin">The admin who is requesting the list</param>
        /// <param name="clientToken">Token used to validate from which client the request originated.</param>
        /// <returns>A list of UserDTOs containing the id and username of all users but ommiting their password.</returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        IList<UserDTO> GetAllUsers(UserDTO admin, string clientToken);

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="requestingUser">The user who requests the deletion, should be either an admin or the user who is to be deleted.</param>
        /// <param name="acountToBeDeletedId">The id of the user who is to be deleted.</param>
        /// <param name="clientToken">Token used to validate from which client the request originated.</param>
        /// <returns>True if the user was successfully deleted, otherwise false</returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool DeleteAccount(UserDTO requestingUser, int acountToBeDeletedId,  string clientToken);
    }
}
