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
        bool CreateAccount(User user);
        User GetAccountInformation(User user);
        bool UpdateAccountInformation(User user);
    }
}
