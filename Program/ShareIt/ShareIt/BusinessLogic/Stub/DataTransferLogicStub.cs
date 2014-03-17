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
        public Stream GetMediaFileStream(Client client, User user, int id, out string fileExtension)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("This is a test stream. If you can read this text you have succesfully consumed the stream.");
            }
            fileExtension = ".txt";
            return stream;
        }

        public bool SaveMedia(MediaItem media, Stream stream)
        {
            return true;
        }
    }
}
