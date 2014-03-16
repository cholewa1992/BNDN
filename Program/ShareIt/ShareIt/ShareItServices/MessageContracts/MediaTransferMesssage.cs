using System;
using System.IO;
using System.ServiceModel;
using BusinessLogicLayer.DTO;

namespace ShareItServices.MessageContracts
{
    /// <summary>
    /// The message contract for transfering Media.
    /// </summary>
    [MessageContract]
    public class MediaTransferMessage : IDisposable
    {
        /// <summary>
        /// The Information about the media being transfered.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public MediaItem Information;
        /// <summary>
        /// The length of the FileByteStream
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public long FileByteStreamLength;
        /// <summary>
        /// The stream containing the binary data of the media being transfered.
        /// </summary>
        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
        /// <summary>
        /// Dispose the MediaTransferMessage closing the FileByteStream.
        /// </summary>
        public void Dispose()
        {
            if (FileByteStream == null) return;
            FileByteStream.Close();
            FileByteStream = null;
        }
    }
}
