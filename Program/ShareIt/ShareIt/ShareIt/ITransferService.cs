using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ShareIt.MessageContracts;

namespace ShareIt
{
    /// <summary>
    /// A service for transfering media.
    /// </summary>
    [ServiceContract]
    public interface ITransferService
    {
        /// <summary>
        /// Download a media.
        /// </summary>
        /// <param name="request">The message contract specifying which media to download.</param>
        /// <returns>A MediaTransferMessage containing information about the media and a stream for downloading it.</returns>
        [OperationContract]
        DownloadResponse DownloadMedia(DownloadRequest request);

        /// <summary>
        /// Upload a media.
        /// </summary>
        /// <param name="media">The UploadRequest containing information about the media being uploaded aswell as a stream which is used for the transfer.</param>
        /// <returns>An UploadStatusMessage specifying wether the upload succeeded or not.</returns>
        [OperationContract]
        UploadStatusMessage UploadMedia(UploadRequest media);
    }
}
