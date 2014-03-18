using System;
using System.Collections.Generic;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.Stub
{
    public class AccessRightLogicStub : IAccessRightLogic
    {
        public bool Purchase(User u, MediaItem m, Client c, DateTime expiration)
        {
            return true;
        }

        public bool Upload(User u, MediaItem m, Client c)
        {
            return true;
        }

        public bool MakeAdmin(User newAdmin)
        {
            return true;
        }

        public bool DeleteAccessRight(AccessRight ar)
        {
            return true;
        }

        public List<AccessRight> GetPurchaseHistory(User u)
        {
            return new List<AccessRight>();
        }

        public List<AccessRight> GetUploadHistory(User u)
        {
            return new List<AccessRight>();
        }

        public bool EditExpiration(AccessRight newAR)
        {
            return true;
        }
    }
}