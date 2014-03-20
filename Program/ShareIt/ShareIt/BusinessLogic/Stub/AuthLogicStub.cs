using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer.Stub
{
    public class AuthLogicStub: IAuthLogic
    {
        public bool CheckUserAccess(int userId, int mediaItemId)
        {
            return true;
        }

        public bool CheckClientAccess(Client client, MediaItem mediaItem)
        {
            return true;
        }

        public bool CheckClientToken(string clientToken)
        {
            return true;
        }

        public bool IsUserAdminOnClient(User user, string clientToken)
        {
            return true;
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