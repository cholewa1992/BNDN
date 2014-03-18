using System.ServiceModel;
using BusinessLogicLayer.DTO;

namespace ShareIt.MessageContracts
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
