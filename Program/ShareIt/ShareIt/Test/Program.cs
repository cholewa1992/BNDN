using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.ServiceReference1;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var clientToken = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

            var accessRightId = 1;

            var admin = new UserDTO
            {
                Id = 1,
                Username = "admin1",
                Password = "password",
                Information = new List<UserInformationDTO>()
            };

            var client = new AccessRightServiceClient();

            client.Delete(admin, accessRightId, clientToken);
        }
    }
}
