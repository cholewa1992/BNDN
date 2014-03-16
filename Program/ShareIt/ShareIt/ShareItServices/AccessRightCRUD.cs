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
        public bool MakeAdmin(User oldAdmin, User newAdmin, Client c)
        {
            var fault = new UnauthorizedUser();
            fault.Message = "The User is not authorized to perform this request.";
            throw new FaultException<UnauthorizedUser>(fault);
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
