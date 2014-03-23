using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

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

        public bool CheckUserExists(User user)
        {
            return true;
        }

        public bool CheckClientExists(Client client)
        {
            return true;
        }
    }
}