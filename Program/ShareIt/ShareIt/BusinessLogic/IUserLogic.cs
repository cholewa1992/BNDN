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
        User GetAccountInformation(int id);
        bool UpdateAccountInformation(User user);
    }
}
