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
        public Stream GetMediaStream(string clientToken, User user, int id, out string fileExtension)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("This is a test stream. If you can read this text you have succesfully consumed the stream.");
            writer.Flush();
            fileExtension = ".txt";
            stream.Position = 0;
            return stream;
        }

        public bool SaveMedia(string clientToken, User owner, MediaItem media, Stream stream)
        {
            return true;
        }

    }
}
