using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using System.Security.Authentication;
using DataAccessLayer;

namespace BusinessLogicLayer.Stub
{
    public class AccessRightLogicStub : IAccessRightInternalLogic
    {

        /// <summary>
        /// Construct a AccessRightLogicStub object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        internal AccessRightLogicStub()
        {

        }

        public bool Purchase(User u, MediaItem m, DateTime expiration, string clientToken)
        {
            return true;
        }

        public bool MakeAdmin(User oldAdmin, User newAdmin, string clientToken)
        {
            return true;
        }

        public bool DeleteAccessRight(User admin, AccessRight ar, string clientToken)
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

        public bool EditExpiration(User u, AccessRight newAR, string clientToken)
        {
            return true;
        }

        public void Dispose()
        {
        }
    }
}
