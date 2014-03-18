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
        bool Purchase(User u, MediaItem m, DateTime expiration);
        bool Upload(User u, MediaItem m);
        bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken);
        bool DeleteAccessRight(User admin, AccessRight ar, string clientToken);
        List<AccessRight> GetPurchaseHistory(User u);
        List<AccessRight> GetUploadHistory(User u);
        bool EditExpiration(User u, AccessRight newAR, string clientToken);
    }
}
