using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ShareItServices.DataContracts;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAccessRightCRUD" in both code and config file together.
    [ServiceContract]
    public interface IAccessRightCRUD
    {
        [OperationContract]
        HttpStatusCode Purchase(User u, MediaItem m, Client c);

        [OperationContract]
        HttpStatusCode Upload(User u, MediaItem m, Client c);

        [OperationContract]
        HttpStatusCode MakeAdmin(User admin, User );

        [OperationContract]
        HttpStatusCode Delete();

        [OperationContract]
        HttpStatusCode GetInfo();

        [OperationContract]
        HttpStatusCode EditExpiration();
    }
}
