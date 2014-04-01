using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using System.Security.Authentication;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public class AccessRightLogic : IAccessRightInternalLogic
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
            Contract.Requires<ArgumentException>(user != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(user.Id > 0);
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));
            
            if (_authLogic.CheckClientToken(clientToken) < 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

            if (_authLogic.CheckUserExists(user) == -1)
            {
                throw new UnauthorizedAccessException("Invalid User credentials!");
            }

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
            Contract.Requires<ArgumentException>(oldAdmin != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(oldAdmin.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(oldAdmin.Username));
            Contract.Requires<ArgumentException>(oldAdmin.Id > 0);
            Contract.Requires<ArgumentException>(newAdminId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));


            var clientId = _authLogic.CheckClientToken(clientToken);
            if (clientId < 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

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
            Contract.Requires<ArgumentException>(admin != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(admin.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(admin.Username));
            Contract.Requires<ArgumentException>(admin.Id > 0);
            Contract.Requires<ArgumentException>(accessRightId > 0);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            if (_authLogic.CheckClientToken(clientToken) < 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

            if (!_authLogic.IsUserAdminOnClient(admin.Id, clientToken))
            {
                throw new UnauthorizedAccessException("User does not have access to perform this operation!");
            }
            //Check if accessRight exists
            _storage.Get<AccessRight>(accessRightId);

            _storage.Delete<AccessRight>(accessRightId);

            return true;
        }

        public List<AccessRightDTO> GetPurchaseHistory(int userId)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(userId > 0);

            try
            {
                _storage.Get<UserAcc>(userId);
            }
            catch (InvalidOperationException e)
            {
                throw new ObjectNotFoundException("User not found.");
            }

            var acessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == userId &&
                x.AccessRightTypeId == (int)AccessRightType.Buyer);

            var accessRights = mapAccessRights(acessRights);

            return accessRights;
        }

        public List<AccessRightDTO> GetUploadHistory(int userId)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(userId > 0);

            try
            {
                _storage.Get<UserAcc>(userId);
            }
            catch (InvalidOperationException e)
            {
                throw new ObjectNotFoundException("User not found.");
            }

            var acessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == userId &&
                x.AccessRightTypeId == (int)AccessRightType.Owner);

            var accessRights = mapAccessRights(acessRights);

            return accessRights;
        }

        public bool EditExpiration(UserDTO u, AccessRightDTO newAR, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(u != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(u.Password));
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(u.Username));
            Contract.Requires<ArgumentException>(u.Id > 0);
            Contract.Requires<ArgumentException>(newAR.Id > 0);
            Contract.Requires<ArgumentException>(newAR.Expiration != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(clientToken));

            if (_authLogic.CheckClientToken(clientToken) < 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

            if (_authLogic.CheckUserExists(u) == -1)
            {
                throw new UnauthorizedAccessException("Invalid User credentials or User does not exist.");
            }

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
                _storage.Update<AccessRight>(oldAR);
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
    }
}
