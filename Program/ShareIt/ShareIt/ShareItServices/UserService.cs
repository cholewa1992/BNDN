using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in both code and config file together.
    public class UserService : IUserService
    {
        public void CreateAccount(User user)
        {
            throw new NotImplementedException();
        }

        public User GetAccountInformation()
        {
            throw new NotImplementedException();
        }

        public void UpdateAccounInformation(User user)
        {
            throw new NotImplementedException();
        }
    }
}
