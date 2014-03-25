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
        List<AccessRightDTO> GetPurchaseHistory(int userId);
        List<AccessRightDTO> GetUploadHistory(int userId);
    }
}
