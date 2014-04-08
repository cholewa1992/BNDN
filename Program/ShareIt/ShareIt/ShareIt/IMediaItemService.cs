using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;

namespace ShareIt
{
    /// <summary>
    /// This interface shows available actions that can be performed on media items
    /// </summary>
    [ServiceContract]
    public interface IMediaItemService
    {
        /// <summary>
        /// Get a media item including all of its information.
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="userId">The id of the user requesting the media item. Null is allowed and can be used if the user is not logged in</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem</returns>
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(MediaItemNotFound))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        MediaItemDTO GetMediaItemInformation(int mediaItemId, int? userId, string clientToken);

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
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(MediaItemNotFound))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> GetMediaItems(int from, int to, string clientToken);

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
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(MediaItemNotFound))]
        [FaultContract(typeof (FaultException))]
        [OperationContract]
        Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> GetMediaItemsByType(int from, int to, MediaItemTypeDTO mediaType, string clientToken);

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
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(MediaItemNotFound))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> SearchMediaItems(int from, int to, string searchKey, string clientToken);

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
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(MediaItemNotFound))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> SearchMediaItemsByType(int from, int to, MediaItemTypeDTO? mediaType, string searchKey, string clientToken);

        /// <summary>
        /// Associates a user with a media item and includes a value from 1-10 representing the rating.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="rating">The rating from 1-10</param>
        /// <param name="clientToken">A token used to verify the client</param>
        /// <exception cref="FaultException">Thrown when something unexpected happens</exception>
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        void RateMediaItem(int userId, int mediaItemId, int rating, string clientToken);

        /// <summary>
        /// Deletes a media item and all of its associations if the user has the right to do so. 
        /// Only admins and owners are allowed to delete media items.
        /// </summary>
        /// <param name="userId">The id of user who wishes to delete a media item</param>
        /// <param name="mediaItemId">The id of the media item to be deleted</param>
        /// <param name="clientToken">A token used to verify the client</param>
        /// <exception cref="FaultException&lt;ArgumentFault&gt;">Thrown when the userId or the mediaItemId is not > 0</exception>
        /// <exception cref="FaultException&lt;ArgumentFault&gt;">Thrown when the clientToken is null</exception>
        /// <exception cref="FaultException&lt;UnauthorizedClient&gt;">Thrown when the clientToken is not accepted</exception>
        /// <exception cref="FaultException&lt;AccessRightNotFound&gt;">Thrown when the requesting user is not allowed to delete the media item</exception>
        /// <exception cref="FaultException">Thrown when something unexpected happens</exception>
        [FaultContract(typeof(ArgumentFault))]
        [FaultContract(typeof(AccessRightNotFound))]
        [FaultContract(typeof(UnauthorizedClient))]
        [FaultContract(typeof(UnauthorizedUser))]
        [FaultContract(typeof(MediaItemNotFound))]
        [FaultContract(typeof(FaultException))]
        [OperationContract]
        void DeleteMediaItem(int userId, int mediaItemId, string clientToken);
        /// <summary>
        /// Update the information of a media item.
        /// </summary>
        /// <param name="user">The user who wishes to update the information.</param>
        /// <param name="media">The media which is to be updated.</param>
        /// <param name="clientToken">A string validating from which client the request originates.</param>
        /// <returns>True if the update succeeded, otherwise false.</returns>
        [OperationContract]
        bool UpdateMediaItemInformation(UserDTO user, MediaItemDTO media, string clientToken);
    }
}

