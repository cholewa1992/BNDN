namespace ShareItServices
{
    public class AuthService: IAuthService
    {
        public string GetAuthToken(string username, string password)
        {
            return "testAuthToken";
        }

        public bool Authenticate(string token)
        {
            return token == "testAuthToken";
        }
    }
}