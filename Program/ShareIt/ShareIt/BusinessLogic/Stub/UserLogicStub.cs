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

        public bool CreateAccount(User user, string clientToken)
        {
            return true;
        }

        public User GetAccountInformation(User requestingUser, User targetUser, string clientToken)
        {
            return targetUser;
        }

        public bool UpdateAccountInformation(User requestingUser, User newUser, string clientToken)
        {
            return true;
        }
    }
}
