using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using ShareItServices.MessageContracts;

namespace ShareItServices
{
    /// <summary>
    /// A service for transfering media.
    /// </summary>
    public class TransferService : ITransferService
    {
        private IBusinessLogicFactory _factory;
        /// <summary>
        /// Construct a TransferService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public TransferService()
        {
            _factory = BusinessLogicFacade.GetBusinessFactory();
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
        /// <param name="request">The message contract specifying which media to download.</param>
        /// <returns>A MediaTransferMessage containing information about the media and a stream for downloading it.</returns>
        public MediaTransferMessage DownloadMedia(DownloadRequest request)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Upload a media.
        /// </summary>
        /// <param name="media">The MediaTransferMessage containing information about the media being uploaded aswell as a stream which is used for the transfer.</param>
        public UploadStatusMessage UploadMedia(MediaTransferMessage media)
        {
            throw new NotImplementedException();
        }
    }
}
