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
        /// <param name="request">The DownloadRequest specifying which media to download.</param>
        /// <returns>A DownloadResponse containing a file extension for the media and a stream for downloading it.</returns>
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        DownloadResponse DownloadMedia(DownloadRequest request);

        /// <summary>
        /// Upload a media.
        /// </summary>
        /// <param name="request">The UploadRequest containing information about the media being uploaded aswell as a stream which is used for the transfer.</param>
        /// <returns>An UploadStatusMessage specifying wether the upload succeeded or not.</returns>
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        UploadResponse UploadMedia(UploadRequest request);
        /// <summary>
        /// Upload a thumbnail and associate it with a media.
        /// </summary>
        /// <param name="request">The ThumbnailUploadRequest which contains the information necessary to upload the thumbnail.</param>
        /// <returns>An UploadStatusMessage specifying wether the upload succeeded or not.</returns>
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        ThumbnailUploadResponse UploadThumbnail(ThumbnailUploadRequest request);
    }
}
