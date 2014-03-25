using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using ShareIt.MessageContracts;

namespace ShareIt
{
    /// <summary>
    /// A service for transfering media.
    /// </summary>
    public class TransferService : ITransferService
    {
        private readonly IBusinessLogicFactory _factory;
        /// <summary>
        /// Construct a TransferService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public TransferService()
        {
            _factory = BusinessLogicFacade.GetTestFactory();
        }
        /// <summary>
        /// Construct a TransferService object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        public TransferService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }
        /// <summary>
        /// Download a media.
        /// </summary>
        /// <param name="request">The DownloadRequest specifying which media to download.</param>
        /// <returns>A DownloadResponse containing the file extension of the media and a stream for downloading it.</returns>
        public DownloadResponse DownloadMedia(DownloadRequest request)
        {
            string fileExtension;
            Stream stream;

            using (var logic = _factory.CreateDataTransferLogic())
            {
               stream = logic.GetMediaStream(request.ClientToken, request.User, request.MediaId, out fileExtension);
            }

            return new DownloadResponse()
            {
                FileByteStream = stream,
                FileByteStreamLength = stream.Length,
                FileExtension = fileExtension
            };

        }

        /// <summary>
        /// Upload a media.
        /// </summary>
        /// <param name="request">The UploadRequest containing information about the media being uploaded aswell as a stream which is used for the transfer.</param>
        /// <returns>An UploadStatusMessage containing information about wether the upload succeeded or not.</returns>
        public UploadResponse UploadMedia(UploadRequest request)
        {
            int result;
            using (var logic = _factory.CreateDataTransferLogic())
            {
                result = logic.SaveMedia(request.ClientToken,request.Owner, request.MetaData, request.FileByteStream);
            }
            return new UploadResponse
            {
                AssignedMediaItemId = result > 0  
            };
        }
    }
}
