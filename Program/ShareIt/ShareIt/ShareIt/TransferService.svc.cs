using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
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
        /// <param name="request">The DownloadRequest specifying which media to download.</param>
        /// <returns>A DownloadResponse containing the file extension of the media and a stream for downloading it.</returns>
        public DownloadResponse DownloadMedia(DownloadRequest request)
        {
            string fileExtension;
            Stream stream;


            try
            {
                using (var logic = _factory.CreateDataTransferLogic())
                {
                    stream = logic.GetMediaStream(request.ClientToken, request.User, request.MediaId, out fileExtension);
                }
            }
            catch (ArgumentNullException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
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

            try
            {
                using (var logic = _factory.CreateDataTransferLogic())
                {
                    result = logic.SaveMedia(request.ClientToken, request.Owner, request.MetaData, request.FileByteStream);
                }
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
            
            return new UploadResponse
            {
                AssignedMediaItemId = result  
            };
        }



        public ThumbnailUploadResponse UploadThumbnail(ThumbnailUploadRequest request)
        {
            string result;

            try
            {
                using (var logic = _factory.CreateDataTransferLogic())
                {
                    result = logic.SaveThumbnail(request.ClientToken, request.Owner, request.MediaId, request.FileExtension, request.FileByteStream);
                }
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault);
            }
            catch (InvalidOperationException e)
            {
                var fault = new MediaItemNotFound();
                fault.Message = e.Message;
                throw new FaultException<MediaItemNotFound>(fault);
            }
            catch (InvalidCredentialException e)
            {
                var fault = new UnauthorizedUser();
                fault.Message = e.Message;
                throw new FaultException<UnauthorizedUser>(fault);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
            
            return new ThumbnailUploadResponse
            {
                ThumbnailURL = result
            };
        }
    }
}
