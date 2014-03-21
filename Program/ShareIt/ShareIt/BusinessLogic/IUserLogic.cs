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
        
        bool CreateAccount(User user, string clientToken);
        User GetAccountInformation(User requestingUser, User targetUser, string clientToken);
        bool UpdateAccountInformation(User requestingUser, User newUser, string clientToken);
    }
}
