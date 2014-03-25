using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IUserLogic
    {
        
        bool CreateAccount(UserDTO user, string clientToken);
        UserDTO GetAccountInformation(UserDTO requestingUser, UserDTO targetUser, string clientToken);
        bool UpdateAccountInformation(UserDTO requestingUser, UserDTO newUser, string clientToken);
    }
}
