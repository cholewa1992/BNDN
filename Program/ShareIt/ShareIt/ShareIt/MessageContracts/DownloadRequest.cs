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
    /// The message contract for requesting a download.
    /// </summary>
    [MessageContract]
    public class DownloadRequest
    {
        /// <summary>
        /// The user requesting the download.
        /// </summary>
        [MessageBodyMember(Order = 1)]
        public User User { get; set; }
        /// <summary>
        /// The Id of the media which the user wishes to download.
        /// </summary>
        [MessageBodyMember(Order = 2)]
        public int MediaId { get; set; }
        /// <summary>
        /// The client from which the request is send.
        /// </summary>
        [MessageBodyMember(Order = 3)]
        public Client Client { get; set; }
    }
}
