using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    public interface IAuthLogic
    {
        bool CheckUserAccess(User user, MediaItem mediaItem, IStorageBridge storage);


        bool CheckClientAccess(Client client, MediaItem mediaItem, IStorageBridge storage);

        bool CheckClientPassword(string clientToken, IStorageBridge storage);

        bool IsUserAdminOnClient(User user, string clientToken, IStorageBridge storage);

        bool CheckUserExists(User user);
        bool CheckUserExists(User user, IStorageBridge storage);

        bool CheckClientExists(Client client);
        bool CheckClientExists(Client client, IStorageBridge storage);
    }
}
