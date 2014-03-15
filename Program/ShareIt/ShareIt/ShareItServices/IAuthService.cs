using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace ShareItServices
{
    [ServiceContract]
    public interface IAuthService
    {
        [OperationContract]
        string GetAuthToken(string username, string password);

        [OperationContract]
        bool Authenticate(string token);

    }
}