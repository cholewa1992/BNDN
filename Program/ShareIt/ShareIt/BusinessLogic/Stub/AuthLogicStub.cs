using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.ClientDTO;

namespace BusinessLogicLayer.Stub
{
    public class AuthLogicStub: IAuthLogic
    {

        public IStorageBridge Storage;
        public IDataTransferLogic Logic;


        public void Dispose()
        {
            Storage.Dispose();
        }

        public bool CheckUserExists(UserDTO user)
        {
            return true;
        }

        public bool CheckUserExists(UserDTO user, string clientToken)
        {
            return true;
        }

        public bool CheckClientExists(Client client)
        {
            return true;
        }
    }
}