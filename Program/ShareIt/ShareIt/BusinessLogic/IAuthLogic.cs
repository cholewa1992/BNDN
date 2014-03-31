using System;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.ClientDTO;

namespace BusinessLogicLayer
{
    public interface IAuthLogic: IDisposable
    {

        bool IsUserAdminOnClient(UserDTO user, string clientToken);

        int CheckUserExists(UserDTO user, string clientToken);

        bool CheckClientExists(Client client);
    }
}
