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
        void CreateAccount(User user);
        User GetAccountInformation(int id);
        void UpdateAccountInformation(User user);
    }
}
