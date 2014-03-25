using System;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.ClientDTO;

namespace BusinessLogicLayer
{
    public interface IAuthLogic: IDisposable
    {
        
        bool CheckUserExists(UserDTO user);

        bool CheckClientExists(Client client);
    }
}
