using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    /// <author>Thomas Dragsbæk (thst@itu.dk)</author>

    public interface IMediaItemLogic : IDisposable
    {
        /// <summary>
        /// Returns a media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="user">The user requesting the media item. Null is allowed and can be used if the user is not logged in</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        MediaItemDTO GetMediaItemInformation(int mediaItemId, UserDTO user, string clientToken);

        /// <summary>
        /// Finds a specific range of media items of a specific media type matching the search keyword.
        /// The media type and the search keyword are optional.
        /// 
        /// E.g. FindMediaItemRange(1, 3, MediaItemType.Book, "money", "token") finds the first 3 books
        /// with some information that contains "money".
        /// 
        /// And FindMediaItemRange(1, 3, null, null, "token") finds the first 3 media items per media type.
        /// </summary>
        /// <param name="from">Where the range must begin. Must be > 0</param>
        /// <param name="to">Where the range must end. Must be > 0</param>
        /// <param name="mediaType">The type of the media item. E.g. MediaItemType.Book for books</param>
        /// <param name="searchKey">The search keyword. This will be matched against all information about all media items</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        /// <exception cref="ArgumentException">Throw when "from" or "to" is &lt; 1</exception>
        /// <exception cref="InvalidOperationException">Thrown when the MediaItemType is not recognized</exception>
        Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> FindMediaItemRange(int @from, int to, MediaItemTypeDTO? mediaType, 
            string searchKey, string clientToken);

        /// <summary>
        /// Associates a user with a media item and includes a value from 1-10 representing the rating.
        /// </summary>
        /// <param name="user">The user who wishes to rate a media item</param>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="rating">The rating from 1-10</param>
        /// <param name="clientToken">A token used to verify the client</param>
        void RateMediaItem(UserDTO user, int mediaItemId, int rating, string clientToken);

        /// <summary>
        /// Deletes a media item and all of its associations if the user has the right to do so. 
        /// Only admins and owners are allowed to delete media items.
        /// </summary>
        /// <param name="user">The user who wishes to delete a media item</param>
        /// <param name="mediaItemId">The id of the media item to be deleted</param>
        /// <param name="clientToken">A token used to verify the client</param>
        /// <exception cref="ArgumentException">Thrown when the userId or the mediaItemId is not > 0</exception>
        /// <exception cref="ArgumentNullException">Thrown when the clientToken is null</exception>
        /// <exception cref="InvalidCredentialException">Thrown when the clientToken is not accepted</exception>
        /// <exception cref="AccessViolationException">Thrown when the requesting user is not allowed to delete the media item</exception>
        void DeleteMediaItem(UserDTO user, int mediaItemId, string clientToken);
        /// <summary>
        /// Update the information about at media.
        /// Only admins and ownsers are allowed to update information of a media item.
        /// </summary>
        /// <param name="user">The user who wishes to update the media information.</param>
        /// <param name="media">The media which is to be updated.</param>
        /// <param name="clientToken">A token used to verify the client.</param>
        void UpdateMediaItem(UserDTO user, MediaItemDTO media, string clientToken);
    }
}
