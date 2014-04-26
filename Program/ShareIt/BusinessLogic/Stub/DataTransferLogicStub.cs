using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.Stub
{
    class DataTransferLogicStub : IDataTransferLogic
    {
        public Stream GetMediaStream(string clientToken, UserDTO user, int mediaId, out string fileExtension)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("This is a test stream. If you can read this text you have succesfully consumed the stream.");
            writer.Flush();
            fileExtension = ".txt";
            stream.Position = 0;
            return stream;
        }

        public int SaveMedia(string clientToken, UserDTO owner, MediaItemDTO media, Stream stream)
        {
            return -1;
        }

        public string SaveThumbnail(string clientToken, UserDTO owner, int mediaId, string fileExtension, Stream fileByteStream)
        {
            return @"http://rentit.itu.dk/rentit08/img/question_mark.PNG";
        }

        public void Dispose()
        {
            
        }
    }
}
