using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Mime;
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
    /// <summary>
    /// This service handles CRUD for Access Rights (Relations between Users and Media Items ie. a purchase).
    /// It also handles making new admins.
    /// </summary>
    /// <Author>Asbjørn Steffensen (afjs@itu.dk)</Author>
    public class AccessRightService : IAccessRightService
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a AccessRightService object which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public AccessRightService()
        {
            _factory = BusinessLogicEntryFactory.GetBusinessFactory();
        }
        /// <summary>
        /// Construct a AccessRightService object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        public AccessRightService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Gives a new user admin rights
        /// </summary>
        /// <param name="oldAdmin">The admin who is trying to upgrade another user to admin</param>
        /// <param name="newAdminId">The id of the user who is the subject of the upgrade</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool MakeAdmin(UserDTO oldAdmin, int newAdminId, string clientToken)
        {
            try
            {
                bool result;

                using (var arl = _factory.CreateAccessRightLogic())
                {
                    result = arl.MakeAdmin(oldAdmin, newAdminId, clientToken);
                }

                return result;
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (UnauthorizedUserException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(e.Message));
            }
            catch (InvalidUserException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (UserNotFoundException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentNullException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason("The "+ e.ParamName +" argument is invalid."));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Deletes an AccessRight (a relation betweeen a User and a MediaItem for instance a purchase)
        /// </summary>
        /// <param name="admin">The admin trying to delete an AccessRight</param>
        /// <param name="accessRightId">The id of the AccessRight to be deleted</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool Delete(UserDTO admin, int accessRightId, string clientToken)
        {
            try
            {
                bool result;

                using (var arl = _factory.CreateAccessRightLogic())
                {
                    result = arl.DeleteAccessRight(admin, accessRightId, clientToken);
                }

                return result;
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (InvalidUserException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (UnauthorizedUserException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(e.Message));
            }
            catch (AccessRightNotFoundException e)
            {
                var fault = new AccessRightNotFound();
                fault.Message = e.Message;
                throw new FaultException<AccessRightNotFound>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentNullException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Edits an already existing AccessRight (a relation betweeen a User and a MediaItem for instance a purchase)
        /// </summary>
        /// <param name="user">The User performing the request</param>
        /// <param name="newAccessRight">The AccessRight containing the new information</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool EditExpiration(UserDTO user, AccessRightDTO newAccessRight, string clientToken)
        {
            try
            {
                bool result;

                using (var arl = _factory.CreateAccessRightLogic())
                {
                    result = arl.EditExpiration(user, newAccessRight, clientToken);
                }

                return result;
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (UnauthorizedUserException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(e.Message));
            }
            catch (InvalidUserException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (AccessRightNotFoundException e)
            {
                var fault = new AccessRightNotFound();
                fault.Message = e.Message;
                throw new FaultException<AccessRightNotFound>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentNullException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Creates a relation between a User and a Media Item (AccessRight)
        ///  that gives a User the right to access a Media Item with the authority of a buyer
        /// </summary>
        /// <param name="user">The User who requests that the AccessRight is created</param>
        /// <param name="mediaItemId">The id of the Media Item which the AccessRight is for</param>
        /// <param name="expiration">The expiration date in the case that the purchase is temporary 
        /// (a rented Media Item). Value is null if the purchase is permanent</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool Purchase(UserDTO user, int mediaItemId, DateTime? expiration, string clientToken)
        {
            try
            {
                bool result;

                using (var arl = _factory.CreateAccessRightLogic())
                {
                    result = arl.Purchase(user, mediaItemId, expiration, clientToken);
                }
                return result;
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (InvalidUserException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(e.Message));
            }
            catch (MediaItemNotFoundException e)
            {
                var fault = new MediaItemNotFound();
                fault.Message = e.Message;
                throw new FaultException<MediaItemNotFound>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentNullException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Gets all the AccessRights where the AccessRightType is buyer for a given User
        /// </summary>
        /// <param name="user">The User performing the request</param>
        /// <param name="userId">The Id of the User whose AccessRights will be returned</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>A List of AccessRights which contains all the AccessRights related to the User 
        /// where the type is buyer</returns>
        public List<AccessRightDTO> GetPurchaseHistory(UserDTO user, int userId, string clientToken)
        {
            try
            {
                List<AccessRightDTO> result;

                using (var arl = _factory.CreateAccessRightLogic())
                {
                    result = arl.GetPurchaseHistory(user, userId, clientToken);
                }

                return result;
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (UnauthorizedUserException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentNullException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (InvalidUserException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (UserNotFoundException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (InvalidOperationException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Gets all the AccessRights where the AccessRightType is owner for a given User
        /// </summary>
        /// <param name="user">The User performing the request</param>
        /// <param name="userId">The Id of the User whose AccessRights will be returned</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>A List of AccessRights which contains all the AccessRights related to the User 
        /// where the type is owner</returns>
        public List<AccessRightDTO> GetUploadHistory(UserDTO user, int userId, string clientToken)
        {
            try
            {
                List<AccessRightDTO> result;

                using (var arl = _factory.CreateAccessRightLogic())
                {
                    result = arl.GetUploadHistory(user, userId, clientToken);
                }

                return result;
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (InvalidUserException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (UserNotFoundException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (InvalidOperationException e)
            {
                var fault = new ObjectNotFound();
                fault.Message = e.Message;
                throw new FaultException<ObjectNotFound>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Checks to see if a User has the right to download a specific Media Item
        /// </summary>
        /// <param name="user">The User who is to be checked to see if he can download the Media Item in question</param>
        /// <param name="mediaItemId">The Id of the Media Item which is to be checked</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the User is allowed to download the Media Item, False if he is not allowed</returns>
        public bool CanDownload(UserDTO user, int mediaItemId, string clientToken)
        {
            bool result;
            using(var logic = _factory.CreateAccessRightLogic())
            {
                try
                {
                    result = logic.CanDownload(user, mediaItemId, clientToken);
                }
                catch (ArgumentNullException e)
                {
                    var fault = new ArgumentFault();
                    fault.Message = e.Message;
                    throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
                }
                catch (ArgumentException e)
                {
                    var msg = e.Message;
                    var fault = new ArgumentFault() {Message = msg};
                    throw new FaultException<ArgumentFault>(fault, new FaultReason(msg));
                }
                catch (InvalidClientException)
                {
                    var msg = "Client token not valid.";
                    var fault = new UnauthorizedClient() {Message = msg};
                    throw new FaultException<UnauthorizedClient>(fault, new FaultReason(msg));
                }
                catch (InvalidUserException)
                {
                    var msg = "Invalid user credentials.";
                    var fault = new UnauthorizedUser() {Message = msg};
                    throw new FaultException<UnauthorizedUser>(fault, new FaultReason(msg));
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    throw new FaultException(new FaultReason(msg));
                }
            }
            return result;
        }
    }
}
