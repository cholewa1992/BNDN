using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IAuthLogic
    {
        bool CheckUserAccess(User user, MediaItem mediaItem);
        bool CheckClientAccess(Client client, MediaItem mediaItem);
        bool CheckClientPassword(Client client);
        bool IsUserAdminOnClient(User user, Client client);
    }
}
