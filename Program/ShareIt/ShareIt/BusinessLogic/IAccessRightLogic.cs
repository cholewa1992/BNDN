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
        bool Purchase(User u, MediaItem m, DateTime expiration, string clientToken);
        bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken);
        bool DeleteAccessRight(User admin, AccessRight ar, string clientToken);
        bool EditExpiration(User u, AccessRight newAR, string clientToken);
    }
}
