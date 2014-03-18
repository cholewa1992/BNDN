using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IAuthLogic
    {
        bool CheckUserAccess(User user, MediaItem mediaItem);
        bool CheckClientAccess(Client client, MediaItem mediaItem);
        bool CheckClientPassword(string clientToken);
        bool IsUserAdminOnClient(User user, string clientToken);
        bool CheckUserExists(User user);
    }
}
