using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Web.Services;
using System.Web.Services.Description;
using BusinessLogicLayer.DTO;

namespace ShareIt
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAuthService" in both code and config file together.
    [ServiceContract]
    public interface IAuthService
    {
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        bool ValidateUser(User user, string clientToken);

        [OperationContract]
        bool CheckClientExists(Client client);
    }
}
