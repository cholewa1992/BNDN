using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ShareIt.MessageContracts
{
    /// <summary>
    /// A message contract specifying what the response to an UploadRequest should look like.
    /// </summary>
    [MessageContract]
    public class UploadResponse
    {
        /// <summary>
        /// The id assigned to the media which was uploaded. Will be -1 if the upload didn't succeed.
        /// </summary>
        [MessageBodyMember(Order = 1)]
        public int AssignedMediaItemId { get; set; }
    }
}
