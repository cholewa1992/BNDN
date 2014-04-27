using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web;
using BusinessLogicLayer.DTO;

namespace ShareIt.MessageContracts
{
    /// <summary>
    /// A message contract for uploading a thumbnail.
    /// </summary>
    [MessageContract]
    public class ThumbnailUploadRequest
    {
        /// <summary>
        /// The client which issued the upload request.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public string ClientToken { get; set; }
        /// <summary>
        /// The id of the media which the thumbnail should be associated with.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public int MediaId { get; set; }
        /// <summary>
        /// The User who owns the thumbnail being uploaded.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public UserDTO Owner { get; set; }
        /// <summary>
        /// The file extension of the thumbnail e.g. .jpg or .png
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
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