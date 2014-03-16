using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareItServices.DataContracts;

namespace BusinessLogicLayer
{
    public interface IAuthLogic
    {
        bool CheckUserAccess(User user, MediaItem mediaItem);
        bool CheckClientAccess(Client client, MediaItem mediaItem);
        bool CheckClientPassword(Client client);
        bool IsUserAdminOnClient(User user, Client client);
    }
}
