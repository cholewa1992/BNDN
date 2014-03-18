using System.IO;
using System.ServiceModel;
using BusinessLogicLayer.DTO;

namespace ShareIt.MessageContracts
{
    /// <summary>
    /// The message contract used when requesting to upload media to the server.
    /// </summary>
    [MessageContract]
    public class UploadRequest 
    {
        /// <summary>
        /// The client which issued the upload request.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public string ClientToken { get; set; }
        /// <summary>
        /// The meta data about the media to be uploaded.
        /// </summary>
        [MessageHeader]
        public MediaItem MetaData { get; set; }
        /// <summary>
        /// The User who owns the media being uploaded.
        /// </summary>
        [MessageHeader]
        public User Owner { get; set; }
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
