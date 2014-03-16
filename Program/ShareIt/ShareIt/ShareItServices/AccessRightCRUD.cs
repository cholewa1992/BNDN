using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AccessRightCRUD" in both code and config file together.
    public class AccessRightCRUD : IAccessRightCRUD
    {
        public HttpStatusCode MakeAdmin(User oldAdmin, User newAdmin, Client c)
        {
            var lol = new UnauthorizedUser();
            lol.Message = "Hey";
            throw new FaultException<UnauthorizedUser>(lol);
        }

        public HttpStatusCode Delete(User admin, AccessRight ar, Client c)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode GetInfo(User u, Client c)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode EditExpiration(User u, AccessRight ar, DateTime newExpiration, Client c)
        {
            throw new NotImplementedException();
        }
    }
}
