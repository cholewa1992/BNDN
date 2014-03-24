﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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

        public bool Purchase(User u, MediaItem m, DateTime expiration, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken) > 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

            if (_authLogic.CheckUserExists(u))
            {
                throw new UnauthorizedAccessException("Invalid User credentials!");
            }

            try
            {
                _storage.Get<Entity>(m.Id);
            }
            catch (InvalidOperationException e)
            {
                throw new InstanceNotFoundException("No Media Item with id "+ m.Id +"was found");
            }

            var newAccessRight = new AccessRight
            {
                Expiration = expiration,
                UserId = u.Id,
                EntityId = m.Id,
                AccessRightTypeId = (int) AccessRightType.Buyer
            };

            _storage.Add(newAccessRight);

            return true;
        }

        public bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken) > 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

            if (_authLogic.IsUserAdminOnClient(oldAdmin.Id, clientToken))
            {
                throw new UnauthorizedAccessException("User does not have access to perform this operation!");
            }

            try
            {
                _storage.Get<UserAcc>(newAdmin.Id);
            }
            catch (InvalidOperationException e)
            {
                throw new InstanceNotFoundException("User not found.");
            }

            var newClientAdmin = new ClientAdmin();

            newClientAdmin.ClientId = _authLogic.CheckClientToken(clientToken);
            newClientAdmin.UserId = newAdmin.Id;

            _storage.Add(newClientAdmin);

            return true;
        }

        public bool DeleteAccessRight(User admin, AccessRightDTO ar, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken) > 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

            if (_authLogic.IsUserAdminOnClient(admin.Id, clientToken))
            {
                throw new UnauthorizedAccessException("User does not have access to perform this operation!");
            }

            _storage.Get<Entity>(ar.Id);

            _storage.Delete<AccessRight>(ar.Id);

            return true;
        }

        public List<AccessRightDTO> GetPurchaseHistory(User u)
        {
            try
            {
                _storage.Get<UserAcc>(u.Id);
            }
            catch (InvalidOperationException e)
            {
                throw new ObjectNotFoundException("User not found.");
            }

            var acessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == u.Id &&
                x.AccessRightTypeId == (int)AccessRightType.Buyer);

            var accessRights = mapAccessRights(acessRights);

            return accessRights;
        }

        public List<AccessRightDTO> GetUploadHistory(User u)
        {
            try
            {
                _storage.Get<UserAcc>(u.Id);
            }
            catch (InvalidOperationException e)
            {
                throw new ObjectNotFoundException("User not found.");
            }

            var acessRights = _storage.Get<AccessRight>()
                .Where(x => x.UserId == u.Id &&
                x.AccessRightTypeId == (int)AccessRightType.Owner);

            var accessRights = mapAccessRights(acessRights);

            return accessRights;
        }

        public bool EditExpiration(User u, AccessRightDTO newAR, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken) > 0)
            {
                throw new InvalidCredentialException("Invalid client token");
            }

            if (_authLogic.CheckUserExists(u))
            {
                throw new UnauthorizedAccessException("Invalid User credentials or User does not exist.");
            }

            if (_authLogic.CheckUserAccess(newAR.UserId, newAR.MediaItemId) != AccessRightType.NoAccess &&
                _authLogic.IsUserAdminOnClient(u.Id, clientToken))
            {
                throw new UnauthorizedAccessException("User does not have access rights to perform this operation!");
            }

            try
            {
                _storage.Get<AccessRight>(newAR.Id);
            }
            catch (InvalidOperationException e)
            {
                throw new InstanceNotFoundException("No access right with id "+ newAR +"was found");
            }

            var newAccessRight = new AccessRight
            {
                Id = newAR.Id,
                EntityId = newAR.MediaItemId,
                AccessRightTypeId = (int) newAR.AccessRightType,
                Expiration = newAR.Expiration,
                UserId = newAR.UserId
            };

            _storage.Update<AccessRight>(newAccessRight);

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
                Id = aR.Id, MediaItemId = aR.EntityId, AccessRightType = 
                (AccessRightType) aR.AccessRightTypeId, Expiration = aR.Expiration, UserId = aR.UserId
            }).ToList();
        }
    }
}