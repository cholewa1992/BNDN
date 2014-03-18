using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace ShareItServices.MessageContracts
{
    /// <summary>
    /// The message contract used when requesting to upload media to the server.
    /// </summary>
    [MessageContract]
    public class UploadRequest : MediaTransferMessage
    {
        /// <summary>
        /// The client which issued the upload request.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public Client Client { get; set; }
    }
}
