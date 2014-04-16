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

        public bool Purchase(UserDTO user, int mediaItemId, DateTime? expiration, string clientToken)
        {
            return true;
        }

        public bool MakeAdmin(UserDTO oldAdmin, int newAdminId, string clientToken)
        {
            return true;
        }

        public bool DeleteAccessRight(UserDTO admin, int accessRightId, string clientToken)
        {
            return true;
        }

        public List<AccessRightDTO> GetPurchaseHistory(int userId)
        {
            return new List<AccessRightDTO>();
        }

        public List<AccessRightDTO> GetUploadHistory(int userId)
        {
            return new List<AccessRightDTO>();
        }

        public bool EditExpiration(UserDTO u, AccessRightDTO newAR, string clientToken)
        {
            return true;
        }

        public List<AccessRightDTO> GetPurchaseHistory(UserDTO user, int userId, string clientToken)
        {
            throw new NotImplementedException();
        }

        public List<AccessRightDTO> GetUploadHistory(UserDTO user, int userId, string clientToken)
        {
            throw new NotImplementedException();
        }

        public bool CanDownload(UserDTO user, int mediaItemId, string clientToken)
        {
            return true;
        }

        public void Dispose()
        {
        }
    }
}
