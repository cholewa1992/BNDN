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
        bool Purchase(UserDTO u, MediaItemDTO m, DateTime expiration, string clientToken);
        bool MakeAdmin(UserDTO oldAdmin, UserDTO newAdmin, string clientToken);
        bool DeleteAccessRight(UserDTO admin, AccessRightDTO ar, string clientToken);
        bool EditExpiration(UserDTO u, AccessRightDTO newAR, string clientToken);
    }
}
