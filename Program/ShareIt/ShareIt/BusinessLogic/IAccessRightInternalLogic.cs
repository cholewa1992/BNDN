using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    internal interface IAccessRightInternalLogic : IAccessRightLogic
    {
        bool Purchase(User u, MediaItem m, DateTime expiration);
        bool Upload(User u, MediaItem m);
        List<AccessRight> GetPurchaseHistory(User u);
        List<AccessRight> GetUploadHistory(User u);
    }
}
