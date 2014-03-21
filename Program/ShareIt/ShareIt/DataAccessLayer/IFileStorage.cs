using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace DataAccessLayer
{
    public interface IFileStorage
    {
        bool SaveFile(Stream stream, int userId, int mediaId, string fileExtension);
    }
}