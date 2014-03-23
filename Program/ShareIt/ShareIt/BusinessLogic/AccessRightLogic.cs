using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public class AccessRightLogic : IAccessRightInternalLogic
    {
        public bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAccessRight(User admin, AccessRight ar, string clientToken)
        {
            throw new NotImplementedException();
        }

        public bool EditExpiration(User u, AccessRight newAR, string clientToken)
        {
            throw new NotImplementedException();
        }

        public bool Purchase(User u, MediaItem m, DateTime expiration)
        {
            throw new NotImplementedException();
        }

        public bool Upload(User u, MediaItem m)
        {
            throw new NotImplementedException();
        }

        public List<AccessRight> GetPurchaseHistory(User u)
        {
            throw new NotImplementedException();
        }

        public List<AccessRight> GetUploadHistory(User u)
        {
            throw new NotImplementedException();
        }
    }
}
