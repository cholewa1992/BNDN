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
        private const string UploadFolder = @"files";
        public bool SaveFile(Stream stream, int userId, int mediaId, string fileExtension)
        {
            var filePath = Path.Combine(UploadFolder, userId.ToString(), mediaId + fileExtension);
            FileStream targetStream;     

            using (targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                try
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
                catch (Exception)
                {
                        
                    throw;
                }
            }
            return true;
        }
    }
}
