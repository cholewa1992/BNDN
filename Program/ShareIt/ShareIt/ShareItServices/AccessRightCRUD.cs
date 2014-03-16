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
        private IAuthLogic _authLogic;
        /// <summary>
        /// Construct a TransferService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public AccessRightCRUD()
        {
            _factory = BusinessLogicFacade.GetBusinessFactory();
            _authLogic = _factory.CreateAuthLogic();
        }
        /// <summary>
        /// Construct a TransferService object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        public AccessRightCRUD(IBusinessLogicFactory factory)
        {
            _factory = factory;
            _authLogic = _factory.CreateAuthLogic();
        }

        public bool MakeAdmin(User oldAdmin, User newAdmin, Client c)
        {
            if (_authLogic.CheckClientPassword(c))
            {
                var fault = new UnauthorizedClient();
                fault.Message = "The Client is not authorized to perform this request.";
                throw new FaultException<UnauthorizedClient>(fault);
            }

            if (_authLogic.IsUserAdminOnClient(oldAdmin, c))
            {
                var fault = new UnauthorizedUser();
                fault.Message = "The User is not authorized to perform this request.";
                throw new FaultException<UnauthorizedUser>(fault);
            }

            //DO SHIT!

            return true;
        }

        public bool Delete(User admin, AccessRight ar, Client c)
        {
            return true;
        }

        public List<AccessRight> GetInfo(User u, Client c)
        {
            throw new NotImplementedException();
        }

        public bool EditExpiration(User u, AccessRight ar, DateTime newExpiration, Client c)
        {
            throw new NotImplementedException();
        }
    }
}
