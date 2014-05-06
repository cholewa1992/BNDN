using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.FaultDataContracts;
using ShareIt;

namespace ShareIt
{
    /// <summary>
    /// This class implements all action that can be performed on media item information.
    /// </summary>
    /// <author>Thomas Dragsbæk (thst@itu.dk)</author>
    public class MediaItemService : IMediaItemService
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a MediaItemInformationService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public MediaItemService()
        {
            _factory = BusinessLogicEntryFactory.GetBusinessFactory(); //Remember to change this
        }

        /// <summary>
        /// Construct a MediaItemInformationService object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the MediaItemInformationService should use for its logic.</param>
        public MediaItemService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get a list of media item information about a given media item.
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="user">The user requesting the media item. Null is allowed and can be used if the user is not logged in</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A list of MediaItemInformation</returns>
        public MediaItemDTO GetMediaItemInformation(int mediaItemId, UserDTO user, string clientToken)
        {
            try
            {
                return _factory.CreateMediaItemLogic().GetMediaItemInformation(mediaItemId, user, clientToken);
            }
            catch (MediaItemNotFoundException e)
            {
                var message = "No media item with id " + mediaItemId + " exists in the database";
                throw new FaultException<MediaItemNotFound>(new MediaItemNotFound
                {
                    Message = message
                }, new FaultReason(message));
            }
            catch (InvalidUserException e)
            {
                var msg = "User credentials not accepted.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault {Message = ae.Message};
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient { Message = e.Message };
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Finds a specific range of media items.
        /// 
        /// E.g. FindMediaItemRange(1, 3, "token") finds the first 3 media items
        /// per media item type.
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> GetMediaItems(int from, int to, string clientToken)
        {
            return SearchMediaItemsByType(from, to, null, null, clientToken);
        }

        /// <summary>
        /// Finds a specific range of media items of a specific media type.
        /// 
        /// E.g. FindMediaItemRange(1, 3, MediaItemType.Book, "token") finds the first 3 books.
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="mediaType">The type of the media item. E.g. MediaItemType.Book for books</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> GetMediaItemsByType(int from, int to, MediaItemTypeDTO mediaType, string clientToken)
        {
            return SearchMediaItemsByType(from, to, mediaType, null, clientToken);
        }

        /// <summary>
        /// Finds a specific range of media items matching the search keyword.
        /// 
        /// E.g. FindMediaItemRange(1, 3, "money", "token") finds the first 3 media items
        /// per media item type with some information that contains "money".
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="searchKey">The search keyword. This will be matched against all information about all media items</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> SearchMediaItems(int from, int to, string searchKey, string clientToken)
        {
            return SearchMediaItemsByType(from, to, null, searchKey, clientToken);
        }

        /// <summary>
        /// Finds a specific range of media items of a specific media type matching the search keyword.
        /// 
        /// E.g. FindMediaItemRange(1, 3, MediaItemType.Book, "money", "token") finds the first 3 books
        /// with some information that contains "money".
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="mediaType">The type of the media item. E.g. MediaItemType.Book for books</param>
        /// <param name="searchKey">The search keyword. This will be matched against all information about all media items</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="FaultException&lt;Argument&gt;">Thrown when from or to is &lt; 1</exception>
        /// <exception cref="FaultException">Thrown when the MediaItemType is not recognized or when something unexpected happens</exception>
        public Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> SearchMediaItemsByType(int from, int to, MediaItemTypeDTO? mediaType, string searchKey, string clientToken)
        {
            try
            {
                return _factory.CreateMediaItemLogic().FindMediaItemRange(@from, to, mediaType, searchKey, clientToken);
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault {Message = ae.Message};
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
            }
            catch (InvalidOperationException e)
            {
                throw new FaultException(new FaultReason("Error when casting the MediaItemType"));
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient {Message = e.Message};
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (MediaItemNotFoundException e)
            {
                var fault = new MediaItemNotFound { Message = e.Message };
                throw new FaultException<MediaItemNotFound>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Associates a user with a media item and includes a value from 1-10 representing the rating.
        /// </summary>
        /// <param name="user">The user who wishes to rate a media item</param>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="rating">The rating from 1-10</param>
        /// <param name="clientToken">A token used to verify the client</param>
        /// <exception cref="FaultException">Thrown when something unexpected happens</exception>
        public void RateMediaItem(UserDTO user, int mediaItemId, int rating, string clientToken)
        {
            try
            {
                _factory.CreateMediaItemLogic().RateMediaItem(user, mediaItemId, rating, clientToken);
            }
            catch (InvalidClientException ae)
            {
                var fault = new UnauthorizedClient { Message = ae.Message };
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(ae.Message));
            }
            catch (InvalidUserException e)
            {
                var msg = "User credentials not accepted.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (ArgumentNullException ae)
            {
                if (ae.ParamName.Equals("user"))
                {
                    string msg = "Log in to rate a media item";
                    var fault = new UnauthorizedUser() {Message = msg};
                    throw new FaultException<UnauthorizedUser>(fault, new FaultReason(msg));
                }
                if (ae.ParamName.Equals("clientToken"))
                {
                    string msg = "Invalid client token";
                    var fault = new UnauthorizedClient {Message = msg};
                    throw new FaultException<UnauthorizedClient>(fault, new FaultReason(msg));
                }

                throw new FaultException(new FaultReason(ae.Message));
            }
            catch (ArgumentException ae)
            {
                var fault = new ArgumentFault {Message = ae.Message};
                throw new FaultException<ArgumentFault>(fault, new FaultReason(ae.Message));
            }
            
            catch (MediaItemNotFoundException e)
            {
                var fault = new MediaItemNotFound { Message = e.Message };
                throw new FaultException<MediaItemNotFound>(fault, new FaultReason(e.Message));
            }
            catch (InvalidOperationException e)
            {
                throw new FaultException(new FaultReason("Error when casting the MediaItemType"));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        /// <summary>
        /// Deletes a media item and all of its associations if the user has the right to do so. 
        /// Only admins and owners are allowed to delete media items.
        /// </summary>
        /// <param name="user">The user who wishes to delete a media item</param>
        /// <param name="mediaItemId">The id of the media item to be deleted</param>
        /// <param name="clientToken">A token used to verify the client</param>
        /// <exception cref="FaultException&lt;ArgumentFault&gt;">Thrown when the userId or the mediaItemId is not > 0</exception>
        /// <exception cref="FaultException&lt;ArgumentFault&gt;">Thrown when the clientToken is null</exception>
        /// <exception cref="FaultException&lt;UnauthorizedClient&gt;">Thrown when the clientToken is not accepted</exception>
        /// <exception cref="FaultException&lt;AccessRightNotFound&gt;">Thrown when the requesting user is not allowed to delete the media item</exception>
        /// <exception cref="FaultException">Thrown when something unexpected happens</exception>
        public void DeleteMediaItem(UserDTO user, int mediaItemId, string clientToken)
        {
            try
            {
                _factory.CreateMediaItemLogic().DeleteMediaItem(user, mediaItemId, clientToken);
            }
            catch (InvalidUserException e)
            {
                var msg = "User credentials not accepted.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (ArgumentException e)
            {
                var fault = new ArgumentFault { Message = e.Message };
                throw new FaultException<ArgumentFault>(fault, new FaultReason(e.Message));
            }
            catch (InvalidClientException e)
            {
                var fault = new UnauthorizedClient { Message = e.Message };
                throw new FaultException<UnauthorizedClient>(fault, new FaultReason(e.Message));
            }
            catch (UnauthorizedUserException e)
            {
                var fault = new UnauthorizedUser {Message = e.Message};
                throw new FaultException<UnauthorizedUser>(fault, new FaultReason(e.Message)); 
            }
            catch (MediaItemNotFoundException e)
            {
                var fault = new MediaItemNotFound { Message = e.Message };
                throw new FaultException<MediaItemNotFound>(fault, new FaultReason(e.Message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
        }

        public bool UpdateMediaItemInformation(UserDTO user, MediaItemDTO media, string clientToken)
        {
            try
            {
                using (var logic = _factory.CreateMediaItemLogic())
                    logic.UpdateMediaItem(user, media, clientToken);
                return true;
            }
            catch (InvalidUserException)
            {
                var msg = "Username and password didn't match.";
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (InvalidClientException)
            {
                var msg = "Client token invalid.";
                throw new FaultException<UnauthorizedClient>(new UnauthorizedClient()
                {
                    Message = msg
                }, new FaultReason(msg));
            }
            catch (UnauthorizedAccessException)
            {
                var message = "User not allowed to update media information for media with id: " + media.Id;
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser()
                {
                    Message = message
                }, new FaultReason(message));
            }
            catch (MediaItemNotFoundException)
            {
                var message = "No media item found with id: " + media.Id;
                throw new FaultException<MediaItemNotFound>(new MediaItemNotFound(){Message = message}, new FaultReason(message));
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message));
            }
                
        }
    }
}
