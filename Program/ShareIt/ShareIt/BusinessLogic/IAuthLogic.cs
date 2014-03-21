using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    public interface IAuthLogic
    {
        bool CheckUserAccess(int userId, int mediaItemId);

        bool CheckClientAccess(Client client, MediaItem mediaItem);

        bool CheckClientToken(string clientToken);

        bool IsUserAdminOnClient(User user, string clientToken);

        bool CheckUserExists(User user);

        bool CheckClientExists(Client client);
    }
}
