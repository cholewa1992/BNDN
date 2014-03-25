using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAccessRightService" in both code and config file together.
    [ServiceContract]
    public interface IAccessRightService
    {
        /// <summary>
        /// Gives a new user admin rights
        /// </summary>
        /// <param name="oldAdmin">The admin who is trying to upgrade another user to admin</param>
        /// <param name="newAdminId">The user who is the subject of the upgrade</param>
        /// <param name="clientToken">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool MakeAdmin(UserDTO oldAdmin, int newAdminId, string clientToken);

        /// <summary>
        /// Deletes an AccessRight (a relation betweeen a User and a MediaItem for instance a purchase)
        /// </summary>
        /// <param name="admin">The admin trying to delete an AccessRight</param>
        /// <param name="accessRightId">The id of the AccessRight to be deleted</param>
        /// <param name="clientToken">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool Delete(UserDTO admin, int accessRightId, string clientToken);

        /// <summary>
        /// Edits an already existing AccessRight (a relation betweeen a User and a MediaItem for instance a purchase)
        /// </summary>
        /// <param name="user">The User performing the request</param>
        /// <param name="newAccessRight">The AccessRight containing the new information</param>
        /// <param name="clientToken">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool EditExpiration(UserDTO user, AccessRightDTO newAccessRight, string clientToken);

        /// <summary>
        /// Creates a new AccessRight (a relation betweeen a User and a MediaItem for instance a purchase) where the 
        /// AccessRightType is buyer.
        /// </summary>
        /// <param name="user">The User performing the request</param>
        /// <param name="mediaItemId">The id of the MediaItem the User is trying to purchase</param>
        /// <param name="expiration">The expiration time of the purchase (if the MediaItem is being rented. 
        /// Value is Null if it is a permanent purchase).</param>
        /// <param name="clientToken">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool Purchase(UserDTO user, int mediaItemId, DateTime expiration, string clientToken);
    }
}
