using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IAccessRightLogic
    {
        bool Purchase(User u, MediaItem m, Client c, DateTime expiration);
        bool Upload(User u, MediaItem m, Client c);
        bool MakeAdmin(User newAdmin);
        bool DeleteAccessRight(AccessRight ar);
        List<AccessRight> GetAccessRightInfo(User u);
        bool EditExpiration(AccessRight oldAR, AccessRight newAR);
    }
}
