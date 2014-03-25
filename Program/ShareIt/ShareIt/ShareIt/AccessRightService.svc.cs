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
using BusinessLogicLayer.FaultDataContracts;

namespace ShareIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AccessRightService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AccessRightService.svc or AccessRightService.svc.cs at the Solution Explorer and start debugging.
    public class AccessRightService : IAccessRightService
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a AccessRightService object which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public AccessRightService()
        {
            _factory = BusinessLogicFacade.GetTestFactory();
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
        /// <param name="clientToken">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool MakeAdmin(User oldAdmin, int newAdminId, string clientToken)
        {
            try
            {
                return _factory.CreateAccessRightLogic().MakeAdmin(oldAdmin, newAdminId, clientToken);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault);
            }
            catch (InstanceNotFoundException e)
            {
                var fault = new MediaItemNotFound();
                fault.Message = e.Message;
                throw new FaultException<MediaItemNotFound>(fault);
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
        /// <param name="clientToken">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool Delete(User admin, int accessRightId, string clientToken)
        {
            try
            {
                return _factory.CreateAccessRightLogic().DeleteAccessRight(admin, accessRightId, clientToken);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
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
        public bool EditExpiration(User u, AccessRightDTO newAR, string clientToken)
        {
            try
            {
                return _factory.CreateAccessRightLogic().EditExpiration(u, newAR, clientToken);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault);
            }
            catch (InstanceNotFoundException e)
            {
                var fault = new AccessRightNotFound();
                fault.Message = e.Message;
                throw new FaultException<AccessRightNotFound>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

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
        public bool Purchase(User user, int mediaItemId, DateTime expiration, string clientToken)
        {
            try
            {
                return _factory.CreateAccessRightLogic().Purchase(user, mediaItemId, expiration, clientToken);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedClient();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedClient>(fault);
            }
            catch (UnauthorizedAccessException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault);
            }
            catch (InstanceNotFoundException e)
            {
                var fault = new MediaItemNotFound();
                fault.Message = e.Message;
                throw new FaultException<MediaItemNotFound>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
