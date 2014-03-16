using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        /// <param name="c">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool MakeAdmin(User oldAdmin, User newAdmin, Client c)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(c))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            if (!_factory.CreateAuthLogic().IsUserAdminOnClient(oldAdmin, c))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            try
            {
                //DO SHIT!

                return true;
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
        /// <param name="c">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool Delete(User admin, AccessRight ar, Client c)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(c))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            if (!_factory.CreateAuthLogic().IsUserAdminOnClient(admin, c))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            try
            {
                //DO SHIT!

                return true;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Returns a list of MediaItems that a given User has purchased or uploaded
        /// </summary>
        /// <param name="u">The user whose AccessRights will be returned</param>
        /// <param name="c">The client from which the request originated</param>
        /// <returns>A list of access rights if the request succeeds. Otherwise it returns a fault.</returns>
        public List<AccessRight> GetInfo(User u, Client c)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(c))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            try
            {
                //DO SHIT!

                return null;
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
        /// <param name="oldAR">The AccessRight to be edited</param>
        /// <param name="newAR">The AccessRight containing the new information</param>
        /// <param name="c">The client from which the request originated</param>
        /// <returns>True if the request succeeds. Otherwise it returns a fault.</returns>
        public bool EditExpiration(User u, AccessRight oldAR, AccessRight newAR, Client c)
        {
            if (!_factory.CreateAuthLogic().CheckClientPassword(c))
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

            if (!_factory.CreateAuthLogic().CheckUserAccess(oldAR.User, oldAR.MediaItem) && 
                !_factory.CreateAuthLogic().IsUserAdminOnClient(u, c))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            try
            {
                //DO SHIT!

                return true;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }
    }
}
