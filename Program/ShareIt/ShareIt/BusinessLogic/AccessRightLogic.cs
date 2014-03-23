using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        public AccessRightLogic(IAuthInternalLogic authLogic, IStorageBridge storage)
        {
            _authLogic = authLogic;
            _storage = storage;
        }

        public bool Purchase(User u, MediaItem m, DateTime expiration, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if (_authLogic.CheckUserExists(u))
            {
                throw new UnauthorizedAccessException();
            }

            _storage.Get<Entity>(m.Id);

            var newAccessRight = new AcessRight();

            newAccessRight.Expiration = expiration;
            newAccessRight.UserId = u.Id;
            newAccessRight.EntityId = m.Id;
            newAccessRight.AccessRightTypeId = 1; //CHECK THIS IS CORRECT!!

            _storage.Add(newAccessRight);

            return true;
        }

        public bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if (_authLogic.IsUserAdminOnClient(oldAdmin, clientToken))
            {
                throw new UnauthorizedAccessException();
            }

            return true;
        }

        public bool DeleteAccessRight(User admin, AccessRight ar, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if (_authLogic.IsUserAdminOnClient(admin, clientToken))
            {
                throw new UnauthorizedAccessException();
            }

            return true;
        }

        public List<AccessRight> GetPurchaseHistory(User u)
        {
            return new List<AccessRight>();
        }

        public List<AccessRight> GetUploadHistory(User u)
        {
            return new List<AccessRight>();
        }

        public bool EditExpiration(User u, AccessRight newAR, string clientToken)
        {
            if (_authLogic.CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if (_authLogic.CheckUserExists(u))
            {
                throw new UnauthorizedAccessException();
            }

            if (_authLogic.CheckUserAccess(newAR.UserId, newAR.MediaItemId) &&
                _authLogic.IsUserAdminOnClient(u, clientToken))
            {
                throw new UnauthorizedAccessException();
            }

            return true;
        }

        public void Dispose()
        {
            _storage.Dispose();
            _authLogic.Dispose();
        }
    }
}
