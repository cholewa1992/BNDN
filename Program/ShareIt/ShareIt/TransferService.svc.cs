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
using BusinessLogicLayer.Exceptions;
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
            _factory = BusinessLogicEntryFactory.GetBusinessFactory();
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
            int mediaId = request.MediaId;

            try
            {
                using (var logic = _factory.CreateDataTransferLogic())
                {
                    stream = logic.GetMediaStream(request.ClientToken, request.User, mediaId, out fileExtension);
                }
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault();
                fault.Message = e.Message;
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (UnauthorizedUserException)
            {
                var message = "User not allowed to download media with id: " + mediaId;
                var unauthorizedUser = new UnauthorizedUser
                {
                    Message = message
                };
                throw new FaultException<UnauthorizedUser>(unauthorizedUser, new FaultReason(message));
            }
            catch (MediaItemNotFoundException)
            {
                var message = "No media found with id: " + mediaId;
                var mediaItemNotFound = new MediaItemNotFound
                {
                    Message = message
                };
                throw new FaultException<MediaItemNotFound>(mediaItemNotFound, new FaultReason(message));
            }
            catch (InvalidClientException)
            {
                var msg = "Client token not accepted.";
                var unauthorizedClient = new UnauthorizedClient
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedClient>(unauthorizedClient, new FaultReason(msg));
            }
            catch (InvalidUserException)
            {
                var msg = "User credentials not accepted.";
                var unauthorizedUser = new UnauthorizedUser
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedUser>(unauthorizedUser, new FaultReason(msg));
            }
            catch (FaultException)
            {
                throw;
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
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (InvalidClientException)
            {
                var msg = "Client token not accepted.";
                var unauthorizedClient = new UnauthorizedClient
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedClient>(unauthorizedClient, new FaultReason(msg));
            }
            catch (InvalidUserException)
            {
                var msg = "User credentials not accepted.";
                var unauthorizedUser = new UnauthorizedUser
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedUser>(unauthorizedUser, new FaultReason(msg));
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
                throw new FaultException<ArgumentFault>(fault, new FaultReason(fault.Message));
            }
            catch (MediaItemNotFoundException)
            {
                var msg = "No media with id: " + request.MediaId + " found.\n" +
                          "There must be a media which the thumbnail should be associated with.";
                var fault = new MediaItemNotFound();
                fault.Message = msg;
                throw new FaultException<MediaItemNotFound>(fault, new FaultReason(msg));
            }
            catch (InvalidClientException)
            {
                var msg = "Client token not accepted.";
                var unauthorizedClient = new UnauthorizedClient
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedClient>(unauthorizedClient, new FaultReason(msg));
            }
            catch (InvalidUserException)
            {
                var msg = "User credentials not accepted.";
                var unauthorizedUser = new UnauthorizedUser
                {
                    Message = msg
                };
                throw new FaultException<UnauthorizedUser>(unauthorizedUser, new FaultReason(msg));
            }
            catch (UnauthorizedUserException)
            {
                var msg = "User must be owner of the media which he attempts to associate a thumbnail with, or user must be admin.";
                var fault = new UnauthorizedUser();
                fault.Message = msg;
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(msg));
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
