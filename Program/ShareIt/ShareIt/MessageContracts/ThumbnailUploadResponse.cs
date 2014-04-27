using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace ShareIt.MessageContracts
{
    /// <summary>
    /// A message contract for the response to the ThumbnailUploadRequest.
    /// </summary>
    [MessageContract]
    public class ThumbnailUploadResponse
    {
        /// <summary>
        /// The URL where the thumbnail can be found after it has been uploaded to the server.
        /// Will be null if the upload failed.
        /// </summary>
        [MessageBodyMember(Order = 1)]
        public string ThumbnailURL { get; set; }
    }
}