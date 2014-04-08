using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareItServices.UserService;

namespace ShareItServices
{
    class Program
    {
        public static void Main(String[] args)
        {
            using (var c = new UserServiceClient())
            {
                var user = new UserDTO()
                {
                    Username = "mat",
                    Password = "123"
                };

                c.CreateAccount(user, "7dac496c534911c0ef47bce1de772502b0d6a6c60b1dbd73c1d3f285f36a0f61");
            }
        }
    }
}
