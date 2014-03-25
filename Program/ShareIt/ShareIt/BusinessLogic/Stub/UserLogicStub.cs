using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer.Stub
{
    class UserLogicStub : IUserLogic
    {

        public bool CreateAccount(UserDTO user, string clientToken)
        {
            return true;
        }

        public UserDTO GetAccountInformation(UserDTO requestingUser, UserDTO targetUser, string clientToken)
        {
            return targetUser;
        }

        public bool UpdateAccountInformation(UserDTO requestingUser, UserDTO newUser, string clientToken)
        {
            return true;
        }
    }
}
