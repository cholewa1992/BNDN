using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Instrumentation;
using BusinessLogicLayer.DTO;
using System.Security.Authentication;
using BusinessLogicLayer.Exceptions;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This class handles CRUD for Access Rights (Relations between Users and Media Items ie. a purchase).
    /// It also handles making new admins.
    /// </summary>
    /// <Author>Asbjørn Steffensen (afjs@itu.dk)</Author>
    internal class AccessRightLogic : IAccessRightInternalLogic
    {
        private readonly IAuthInternalLogic _authLogic;
        private readonly IStorageBridge _storage;

        /// <summary>
        /// Construct a AccessRightLogic object which uses a specified IBusinessLogicFactory.
        /// </summary>
        /// <param name="authLogic">An instance of IAuthInternalLogic used for validating users and clients</param>
        /// <param name="storage">An instance of IStorageBridge used for storing changes to the database</param>
        internal AccessRightLogic(IAuthInternalLogic authLogic, IStorageBridge storage)
        {
            _authLogic = authLogic;
            _storage = storage;
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
        /// <returns></returns>
        public bool Purchase(UserDTO user, int mediaItemId, DateTime? expiration, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            ValidateClientToken(clientToken);

            user.Id = ValidateUser(user);

            try
            {
                _storage.Get<Entity>(mediaItemId);
            }
            catch (InvalidOperationException e)
            {
                throw new MediaItemNotFoundException("No Media Item with id "+ mediaItemId + "was found");
            }

            var newAccessRight = new AccessRight
            {
                Expiration = expiration,
                UserId = user.Id,
                EntityId = mediaItemId,
                AccessRightTypeId = (int) AccessRightType.Buyer
            };

            _storage.Add(newAccessRight);

            return true;
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
            //Preconditions
            Contract.Requires<ArgumentNullException>(oldAdmin != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(oldAdmin.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(oldAdmin.Username));
            Contract.Requires<ArgumentException>(newAdminId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            oldAdmin.Id = ValidateUser(oldAdmin);
            var clientId = ValidateClientToken(clientToken);

            if (!_authLogic.IsUserAdminOnClient(oldAdmin.Id, clientToken))
            {
                throw new UnauthorizedUserException("User does not have access to perform this operation!");
            }

            try
            {
                _storage.Get<UserAcc>(newAdminId);
            }
            catch (InvalidOperationException e)
            {
                throw new UserNotFoundException("No user found with id: " + newAdminId);
            }

            //return if already admin.
            if (_authLogic.IsUserAdminOnClient(newAdminId, clientToken))
                return true;

            var newClientAdmin = new ClientAdmin();

            newClientAdmin.ClientId = clientId;
            newClientAdmin.UserId = newAdminId;

            _storage.Add(newClientAdmin);

            return true;
        }

        /// <summary>
        /// Deletes an AccessRight (a relation betweeen a User and a MediaItem for instance a purchase)
        /// </summary>
        /// <param name="admin">The admin trying to delete an AccessRight</param>
        /// <param name="accessRightId">The id of the AccessRight to be deleted</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool DeleteAccessRight(UserDTO admin, int accessRightId, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(admin != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(admin.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(admin.Username));
            Contract.Requires<ArgumentException>(accessRightId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            ValidateClientToken(clientToken);
            admin.Id = ValidateUser(admin);

            if (!_authLogic.IsUserAdminOnClient(admin.Id, clientToken))
            {
                throw new UnauthorizedUserException("User does not have access to perform this operation!");
            }
            //Check if accessRight exists
            try
            {
                _storage.Get<AccessRight>(accessRightId);
            }
            catch(InvalidOperationException e)
            {
                throw new AccessRightNotFoundException("No access right with id " + accessRightId + "was found");
            }
            

            _storage.Delete<AccessRight>(accessRightId);

            return true;
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
            //Preconditions
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            ValidateClientToken(clientToken);
            user.Id = ValidateUser(user);

            if (user.Id != userId &&
                !_authLogic.IsUserAdminOnClient(user.Id, clientToken))
            {
                throw new UnauthorizedUserException("User does not have access rights to perform this operation!");
            }
            
            try
            {
                _storage.Get<UserAcc>(userId);
            }
            catch (InvalidOperationException e)
            {
                throw new UserNotFoundException("User with id: "+ user.Id +" not found.");
            }

            var accessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == userId &&
                x.AccessRightTypeId == (int)AccessRightType.Buyer);

            var accessRightDTOs = mapAccessRights(accessRights);

            return accessRightDTOs;
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
            //Preconditions
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            ValidateClientToken(clientToken);

            try
            {
                _storage.Get<UserAcc>(userId);
            }
            catch (InvalidOperationException)
            {
                throw new UserNotFoundException("User with id: " + userId + " not found.");
            }

            var accessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == userId &&
                x.AccessRightTypeId == (int)AccessRightType.Owner);

            var accessRightDTOs = mapAccessRights(accessRights);

            return accessRightDTOs;
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
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            //Check clientToken and User
            if(_authLogic.CheckClientToken(clientToken) == -1)
                throw new InvalidClientException();
            int userId = _authLogic.CheckUserExists(user);
            if (userId == -1)
                throw new InvalidUserException();
            //Admins can always download.
            if (_authLogic.IsUserAdminOnClient(userId, clientToken))
                return true;
            //If access right isn't NoAccess, allow download.
            if (_authLogic.CheckUserAccess(userId, mediaItemId) != AccessRightType.NoAccess)
                return true;
            //Else don't allow download.
            return false;
        }

        /// <summary>
        /// Edits an already existing AccessRight (a relation betweeen a User and a MediaItem for instance a purchase)
        /// </summary>
        /// <param name="user">The User performing the request</param>
        /// <param name="newAccessRight">The AccessRight containing the new information</param>
        /// <param name="clientToken">The token of the client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool EditExpiration(UserDTO u, AccessRightDTO newAR, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(u != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(u.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(u.Username));
            Contract.Requires<ArgumentException>(newAR.Id > 0);
            Contract.Requires<ArgumentNullException>(newAR.Expiration != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            ValidateClientToken(clientToken);

            u.Id = ValidateUser(u);

            if (_authLogic.CheckUserAccess(u.Id, newAR.MediaItemId) != AccessRightType.NoAccess &&
                !_authLogic.IsUserAdminOnClient(u.Id, clientToken))
            {
                throw new UnauthorizedUserException("User does not have access rights to perform this operation!");
            }

            AccessRight oldAR = null;

            try
            {
                oldAR =_storage.Get<AccessRight>(newAR.Id);
            }
            catch (InvalidOperationException)
            {
                throw new AccessRightNotFoundException("No access right with id "+ newAR +"was found");
            }

            if (oldAR != null)
            {
                oldAR.Expiration = newAR.Expiration;
                _storage.Update(oldAR);
            }
            return true;
        }

        public void Dispose()
        {
            _storage.Dispose();
            _authLogic.Dispose();
        }

        private List<AccessRightDTO> mapAccessRights(IEnumerable<AccessRight> acessRights)
        {
            return acessRights.Select(aR => new AccessRightDTO
            {
                Id = aR.Id, 
                MediaItemId = aR.EntityId,
                Expiration = aR.Expiration,
                UserId = aR.UserId
            }).ToList();
        }

        /// <summary>
        /// Validate credentials of a clientToken
        /// </summary>
        /// <param name="clientToken">The clientToken to validate.</param>
        /// <returns>The id of the client if it was validated.</returns>
        /// <exception cref="InvalidCredentialException">If the clientToken was not accepted.</exception>
        private int ValidateClientToken(string clientToken)
        {
            var result = _authLogic.CheckClientToken(clientToken);
            if (result == -1)
                throw new InvalidClientException("Invalid client token");
            return result;
        }

        /// <summary>
        /// Validate credentials of a user.
        /// </summary>
        /// <param name="user">The user whose credentials are to be validated.</param>
        /// <returns>The id of the user if his credentials are validated.</returns>
        /// <exception cref="UnauthorizedAccessException">If the user's credentials aren't validated.</exception>
        private int ValidateUser(UserDTO user)
        {
            var result = _authLogic.CheckUserExists(user);

            if (result == -1)
                throw new InvalidUserException("Invalid User credentials or User does not exist.");

            return result;
        }
    }
}
