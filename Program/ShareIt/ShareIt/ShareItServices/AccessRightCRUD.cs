using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Security.Authentication;
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
            try
            {
                return _factory.CreateAccessRightLogic().MakeAdmin(oldAdmin, newAdmin, clientToken);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
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
            try
            {
                return _factory.CreateAccessRightLogic().DeleteAccessRight(admin, ar, clientToken);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
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
            try
            {
                return _factory.CreateAccessRightLogic().EditExpiration(u, newAR, clientToken);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
