using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AccessRightCRUD" in both code and config file together.
    public class AccessRightCRUD : IAccessRightCRUD
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a AccessRightCRUD Service which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public AccessRightCRUD()
        {
            _factory = BusinessLogicFacade.GetBusinessFactory();
        }
        /// <summary>
        /// Construct a AccessRightCRUD Service object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        public AccessRightCRUD(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Gives a new user admin rights
        /// </summary>
        /// <param name="oldAdmin">The admin who is trying to upgrade another user to admin</param>
        /// <param name="newAdmin">The user who is the subject of the upgrade</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(clientToken))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            if (!_factory.CreateAuthLogic().IsUserAdminOnClient(oldAdmin, clientToken))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            try
            {
                return _factory.CreateAccessRightLogic().MakeAdmin(newAdmin);
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
        /// <param name="ar">The AccessRight to be deleted</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool Delete(User admin, AccessRight ar, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(clientToken))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            if (!_factory.CreateAuthLogic().IsUserAdminOnClient(admin, clientToken))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            try
            {
                return _factory.CreateAccessRightLogic().DeleteAccessRight(ar);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Returns a list of MediaItems that a given User has purchased (AccessRights with type buyer)
        /// </summary>
        /// <param name="requestingUser">The User performing the request</param>
        /// <param name="targetUser">The user whose AccessRights will be returned</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns>A list of access rights if the request succeeds. Otherwise it returns a fault.</returns>
        public List<AccessRight> GetPurchaseHistory(User requestingUser, User targetUser, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(clientToken))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            if (requestingUser != targetUser &&
                !_factory.CreateAuthLogic().IsUserAdminOnClient(requestingUser, clientToken))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            try
            {
                return _factory.CreateAccessRightLogic().GetPurchaseHistory(targetUser);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Returns a list of MediaItems that a given User has uploaded (AccessRights with type owner)
        /// </summary>
        /// <param name="u">The user whose AccessRights will be returned</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns>A list of access rights if the request succeeds. Otherwise it returns a fault.</returns>
        public List<AccessRight> GetUploadHistory(User u, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(clientToken))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            try
            {
                return _factory.CreateAccessRightLogic().GetUploadHistory(u);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Edits an already existing AccessRight (a relation betweeen a User and a MediaItem for instance a purchase)
        /// </summary>
        /// <param name="u">The User performing the request</param>
        /// <param name="newAR">The AccessRight containing the new information</param>
        /// <param name="clientToken">Token used to validate the client</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool EditExpiration(User u, AccessRight newAR, string clientToken)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(clientToken))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            if (!_factory.CreateAuthLogic().CheckUserExists(u))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            if (!_factory.CreateAuthLogic().CheckUserAccess(newAR.User, newAR.MediaItem) &&
                !_factory.CreateAuthLogic().IsUserAdminOnClient(u, clientToken))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            try
            {
                return _factory.CreateAccessRightLogic().EditExpiration(newAR);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
