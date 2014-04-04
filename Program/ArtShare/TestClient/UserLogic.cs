using TestClient.UserService;

namespace TestClient
{
    public class UserLogic
    {
        public bool CreateAccount()
        {
            var token = "7dac496c534911c0ef47bce1de772502b0d6a6c60b1dbd73c1d3f285f36a0f61";

            var user = new UserDTO()
            {
                Username = "Mathias2",
                Password = "password"
            };


            bool result;

            using (var us = new UserServiceClient())
            {
                result = us.CreateAccount(user, token);
            }

            return result;
        } 
    }
}