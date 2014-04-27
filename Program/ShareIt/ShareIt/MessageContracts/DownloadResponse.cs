using System;
using System.IO;
using System.ServiceModel;
using BusinessLogicLayer.DTO;

namespace ShareIt.MessageContracts
{
    /// <summary>
    /// The message contract for the response to a DownloadRequest
    /// </summary>
    [MessageContract]
    public class DownloadResponse : IDisposable
    {
        /// <summary>
        /// The file extension of the media being downloaded.
        /// </summary>
        [MessageHeader]
        public string FileExtension { get; set; }
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
