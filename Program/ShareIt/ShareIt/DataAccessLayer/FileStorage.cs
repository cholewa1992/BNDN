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
        private readonly string _physicalPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        private readonly string _webPath = @"http://rentit.itu.dk/rentit08/";
        /// <summary>
        /// Save a stream to the disk as a file.
        /// </summary>
        /// <param name="stream">The stream to save as a file.</param>
        /// <param name="userId">The id of the user who uploaded the file.</param>
        /// <param name="mediaId">The id of the file's meta data.</param>
        /// <param name="fileExtension">The extension of the file.</param>
        /// <returns>The path to where the file was saved on the disk.</returns>
        public string SaveFile(Stream stream, int userId, int mediaId, string fileExtension)
        {
            var directoryPath = Path.Combine(_physicalPath,"files", "user_" + userId);
            var filePath = Path.Combine(directoryPath, mediaId + fileExtension);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            WriteStreamToDisk(filePath, stream);
            return filePath;
        }

        public Stream ReadFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if(!fileInfo.Exists)
               throw new FileNotFoundException();
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public string SaveThumbnail(Stream fileByteStream, int mediaId, string fileExtension)
        {
            var directoryPath = Path.Combine(_physicalPath, "img");
            var thumbnailName = "thumbnail_" + mediaId + fileExtension;
            var filePath = Path.Combine(directoryPath, thumbnailName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            WriteStreamToDisk(filePath, fileByteStream);
            return _webPath + "/img/" + thumbnailName;
        }

        /// <summary>
        /// Writes a stream to the disk at the given filepath.
        /// </summary>
        /// <param name="filePath">The path where the file should be written.</param>
        /// <param name="stream">The stream which should be written.</param>
        private void WriteStreamToDisk(string filePath, Stream stream)
        {
            FileStream targetStream;
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
        }
    }
}
