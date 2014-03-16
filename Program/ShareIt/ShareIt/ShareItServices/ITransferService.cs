﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using ShareItServices.MessageContracts;

namespace ShareItServices
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
        MediaTransferMessage DownloadMedia(DownloadRequest request);
        /// <summary>
        /// Upload a media.
        /// </summary>
        /// <param name="media">The MediaTransferMessage containing information about the media being uploaded aswell as a stream which is used for the transfer.</param>
        [OperationContract]

        UploadStatusMessage UploadMedia(MediaTransferMessage media);
    }
}