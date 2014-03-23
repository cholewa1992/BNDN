using System;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    public interface IAuthLogic: IDisposable
    {
        
        bool CheckUserExists(User user);

        bool CheckClientExists(Client client);
    }
}
