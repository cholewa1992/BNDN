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
        [MessageHeader(MustUnderstand = true)]
        public User User { get; set; }
        /// <summary>
        /// The Id of the media which the user wishes to download.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public int MediaId { get; set; }
    }
}
