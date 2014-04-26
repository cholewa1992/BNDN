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

        public UserDTO GetAccountInformation(UserDTO requestingUser, int targetUserId, string clientToken)
        {
            return new UserDTO();
        }

        public bool UpdateAccountInformation(UserDTO requestingUser, UserDTO newUser, string clientToken)
        {
            return true;
        }

        public IList<UserDTO> GetAllUsers(UserDTO admin, string clientToken)
        {
            return new List<UserDTO>()
            {
                new UserDTO()
                {
                    Id = 1,
                    Username = "firstUser"
                },
                new UserDTO()
                {
                    Id = 2,
                    Username = "secondUser"
                }
            };
        }

        public bool DeleteUser(UserDTO requestingUser, int userToBeDeletedId, string clientToken)
        {
            return true;
        }

        public void Dispose()
        {
            
        }
    }
}
