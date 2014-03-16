using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using ShareItServices.DataContracts;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AuthService" in both code and config file together.
    public class AuthService : IAuthService
    {

        private readonly IBusinessLogicFactory _factory = BusinessLogicFacade.GetBusinessFactory();


        public AuthService() {}


        public AuthService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }


        public HttpStatusCode CheckAccess(User user, Client client)
        {
            try
            {
                _factory.CreateAuthLogic();
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.Accepted;
        }

        
    }
}
