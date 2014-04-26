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
    internal class AccessRightLogic : IAccessRightInternalLogic
    {
        private readonly IAuthInternalLogic _authLogic;
        private readonly IStorageBridge _storage;

        /// <summary>
        /// Construct a AccessRightLogicStub object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="authLogic"></param>
        /// <param name="storage"></param>
        internal AccessRightLogic(IAuthInternalLogic authLogic, IStorageBridge storage)
        {
            _authLogic = authLogic;
            _storage = storage;
        }

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
                throw new InstanceNotFoundException("No Media Item with id "+ mediaItemId +"was found");
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
                throw new UnauthorizedAccessException("User does not have access to perform this operation!");
            }

            try
            {
                _storage.Get<UserAcc>(newAdminId);
            }
            catch (InvalidOperationException e)
            {
                throw new InstanceNotFoundException("No user found with id: " + newAdminId);
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
                throw new UnauthorizedAccessException("User does not have access to perform this operation!");
            }
            //Check if accessRight exists
            try
            {
                _storage.Get<AccessRight>(accessRightId);
            }
            catch(InvalidOperationException e)
            {
                throw new InstanceNotFoundException("No access right with id " + accessRightId + "was found");
            }
            

            _storage.Delete<AccessRight>(accessRightId);

            return true;
        }

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
                throw new UnauthorizedAccessException("User does not have access rights to perform this operation!");
            }
            
            try
            {
                _storage.Get<UserAcc>(userId);
            }
            catch (InvalidOperationException e)
            {
                throw new ObjectNotFoundException("User with id: "+ user.Id +" not found.");
            }

            var accessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == userId &&
                x.AccessRightTypeId == (int)AccessRightType.Buyer);

            var accessRightDTOs = mapAccessRights(accessRights);

            return accessRightDTOs;
        }

        public List<AccessRightDTO> GetUploadHistory(UserDTO user, int userId, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            ValidateClientToken(clientToken);
            user.Id = ValidateUser(user);

            try
            {
                _storage.Get<UserAcc>(userId);
            }
            catch (InvalidOperationException e)
            {
                throw new ObjectNotFoundException("User with id: " + user.Id + " not found.");
            }

            var accessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == userId &&
                x.AccessRightTypeId == (int)AccessRightType.Owner);

            var accessRightDTOs = mapAccessRights(accessRights);

            return accessRightDTOs;
        }

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
                throw new UnauthorizedAccessException("User does not have access rights to perform this operation!");
            }

            AccessRight oldAR = null;

            try
            {
                oldAR =_storage.Get<AccessRight>(newAR.Id);
            }
            catch (InvalidOperationException e)
            {
                throw new InstanceNotFoundException("No access right with id "+ newAR +"was found");
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
                throw new InvalidCredentialException("Invalid client token");
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
                throw new UnauthorizedAccessException("Invalid User credentials or User does not exist.");

            return result;
        }
    }
}
