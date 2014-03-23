using System;
using System.Collections.Generic;
using System.Security.Authentication;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer.Stub
{
    public class AccessRightLogicStub : IAccessRightLogic
    {

        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a AccessRightLogicStub which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public AccessRightLogicStub()
        {
            _factory = BusinessLogicFacade.GetTestFactory();
        }
        /// <summary>
        /// Construct a AccessRightLogicStub object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        public AccessRightLogicStub(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        public bool Purchase(User u, MediaItem m, DateTime expiration)
        {
            return true;
        }

        public bool Upload(User u, MediaItem m)
        {
            return true;
        }

        public bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if (!_factory.CreateAuthLogic().IsUserAdminOnClient(oldAdmin, clientToken))
            {
                throw new UnauthorizedAccessException();
            }

            return true;
        }

        public bool DeleteAccessRight(User admin, AccessRight ar, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if (!_factory.CreateAuthLogic().IsUserAdminOnClient(admin, clientToken))
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
            if (!_factory.CreateAuthLogic().CheckClientToken(clientToken))
            {
                throw new InvalidCredentialException();
            }

            if (!_factory.CreateAuthLogic().CheckUserExists(u))
            {
                throw new UnauthorizedAccessException();
            }

            if (!_factory.CreateAuthLogic().CheckUserAccess(newAR.UserId, newAR.MediaItemId) &&
                !_factory.CreateAuthLogic().IsUserAdminOnClient(u, clientToken))
            {
                throw new UnauthorizedAccessException();
            }

            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}