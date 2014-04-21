using System;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.ClientDTO;

namespace BusinessLogicLayer.Stub
{
    public class AuthLogicStub: IAuthInternalLogic
    {

        public IStorageBridge Storage;
        public IDataTransferLogic Logic;


        public void Dispose()
        {

        }

        public AccessRightType CheckUserAccess(int userId, int mediaItemId)
        {
            return AccessRightType.Buyer;
        }

        public int CheckClientToken(string clientToken)
        {
            return 1;
        }

        public bool IsUserAdminOnClient(int userId, string clientToken)
        {
            return true;
        }

        public DateTime? GetBuyerExpirationDate(int userId, int mediaItemId)
        {
            return new DateTime(01, 01, 01);
        }

        public int CheckUserExists(UserDTO user)
        {
            return 1;
        }

        public bool IsUserAdminOnClient(UserDTO user, string clientToken)
        {
            return true;
        }

        public int CheckUserExists(UserDTO user, string clientToken)
        {
            return 1;
        }

        public bool CheckClientExists(Client client)
        {
            return true;
        }
    }
}