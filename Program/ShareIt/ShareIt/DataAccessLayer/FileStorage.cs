using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class FileStorage : IFileStorage
    {
        public string SaveFile(Stream stream, int userId, int mediaId, string fileExtension)
        {
            var apPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            var directoryPath = Path.Combine(apPath,"files", "user_" + userId);
            var filePath = Path.Combine(directoryPath, mediaId + fileExtension);
            FileStream targetStream;
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            using (targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                const int bufferLength = 65000;
                var buffer = new byte[bufferLength];
                int count;
                while ((count = stream.Read(buffer, 0, bufferLength)) > 0)
                {
                    targetStream.Write(buffer, 0, count);
                }
                stream.Close();
            }
            return filePath;
        }

        public Stream ReadFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if(!fileInfo.Exists)
               throw new FileNotFoundException();
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
