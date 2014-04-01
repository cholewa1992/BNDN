using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IAccessRightLogic : IDisposable
    {
        bool Purchase(UserDTO user, int mediaItemId, DateTime? expiration, string clientToken);
        bool MakeAdmin(UserDTO oldAdmin, int newAdminId, string clientToken);
        bool DeleteAccessRight(UserDTO admin, int accessRightId, string clientToken);
        bool EditExpiration(UserDTO u, AccessRightDTO newAR, string clientToken);
    }
}
