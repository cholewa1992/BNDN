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
        bool Purchase(User user, int mediaItemId, DateTime expiration, string clientToken);
        bool MakeAdmin(User oldAdmin, int newAdminId, string clientToken);
        bool DeleteAccessRight(User admin, int accessRightId, string clientToken);
        bool EditExpiration(User u, AccessRightDTO newAR, string clientToken);
    }
}
