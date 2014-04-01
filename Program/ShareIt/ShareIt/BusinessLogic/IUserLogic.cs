using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IUserLogic : IDisposable
    {
        
        bool CreateAccount(UserDTO user, string clientToken);
        UserDTO GetAccountInformation(UserDTO requestingUser, int targetUserId, string clientToken);
        bool UpdateAccountInformation(UserDTO requestingUser, UserDTO newUser, string clientToken);
        IList<UserDTO> GetAllUsers(UserDTO admin, string clientToken);
        bool DeleteUser(UserDTO requestingUser, int userToBeDeletedId, string clientToken);
    }
}
