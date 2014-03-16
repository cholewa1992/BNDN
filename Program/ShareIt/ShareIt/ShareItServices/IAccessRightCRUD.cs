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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAccessRightCRUD" in both code and config file together.
    [ServiceContract]
    public interface IAccessRightCRUD
    {
        //[OperationContract]
        //HttpStatusCode Purchase(User u, MediaItem m, Client c, DateTime expiration);

        //[OperationContract]
        //HttpStatusCode Upload(User u, MediaItem m, Client c);

        [FaultContract(typeof(UnauthorizedUser))]
        [OperationContract]
        HttpStatusCode MakeAdmin(User oldAdmin, User newAdmin, Client c);

        [OperationContract]
        HttpStatusCode Delete(User admin, AccessRight ar, Client c);

        [OperationContract]
        HttpStatusCode GetInfo(User u, Client c);

        [OperationContract]
        HttpStatusCode EditExpiration(User u, AccessRight ar, DateTime newExpiration, Client c);
    }
}
