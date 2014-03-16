using System;
using System.IO;
using System.ServiceModel;

namespace ShareItServices.MessageContracts
{
    [MessageContract]
    public class MediaTransferMessage : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public MediaItem Information;
        [MessageHeader(MustUnderstand = true)]
        public long Length;
        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
        public void Dispose()
        {
            if (FileByteStream == null) return;
            FileByteStream.Close();
            FileByteStream = null;
        }
    }
}
