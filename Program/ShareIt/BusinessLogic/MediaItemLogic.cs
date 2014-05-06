using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.ServiceModel;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This class handles searching, rating, getting, updating and deleting media items.
    /// </summary>
    /// <author>Thomas Dragsbæk (thst@itu.dk), Nicki Jørgensen (nhjo@itu.dk)</author>
    internal class MediaItemLogic : IMediaItemInternalLogic
    {
        private readonly IStorageBridge _storage;
        public IFileStorage FileStorage { get; set; }
        private readonly IAuthInternalLogic _authLogic;

        internal MediaItemLogic(IStorageBridge storage, IAuthInternalLogic authLogic)
        {
            _storage = storage;
            _authLogic = authLogic;
            FileStorage = new FileStorage();
        }

        /// <summary>
        /// Returns a media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="user">The </param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItemDTO GetMediaItemInformation(int mediaItemId, UserDTO user, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            int clientId = _authLogic.CheckClientToken(clientToken);
            if (clientId == -1)
            {
                throw new InvalidClientException("Invalid client token");
            }

            Entity entity;

            try
            {
                entity = _storage.Get<Entity>().First(foo => foo.Id == mediaItemId && foo.ClientId == clientId);
            }
            catch (Exception e)
            {
                throw new MediaItemNotFoundException();
            }

            var mediaItem = new MediaItemDTO
            {
                Id = entity.Id,
                Information = new List<MediaItemInformationDTO>(),
                Type = (MediaItemTypeDTO)entity.TypeId,
                FileExtension = Path.GetExtension(entity.FilePath),
                Owner = new UserDTO()
                {
                    
                }
            };
            var ownerRights = entity.AccessRight.SingleOrDefault(x => x.AccessRightTypeId == (int)AccessRightType.Owner);
            if (ownerRights != null)
            {
                int ownerId = ownerRights.UserId;
                string ownerName = ownerRights.UserAcc.Username;
                mediaItem.Owner = new UserDTO()
                {
                    Id = ownerId,
                    Username = ownerName
                };
            }
            
            var informationList = new List<MediaItemInformationDTO>();

            // Add UserInformation to the temporary list object
            foreach (var e in entity.EntityInfo)
            {

                informationList.Add(new MediaItemInformationDTO()
                {
                    Type = (InformationTypeDTO)e.EntityInfoTypeId,
                    Data = e.Data,
                    Id = e.Id
                });
            }

            if (user != null)
            {
                int userId = _authLogic.CheckUserExists(user);
                if (userId == -1)
                {
                    throw new InvalidUserException();
                }
                try
                {
                    DateTime? date = _authLogic.GetBuyerExpirationDate(userId, mediaItem.Id);
                    mediaItem.ExpirationDate = date;
                }
                catch (InstanceNotFoundException e)
                {
                    //No expiration date found. Don't do anything else than NOT adding the information
                }
            }

            try
            {
                int numberOfRatings;
                var avgRating = GetAverageRating(mediaItemId, out numberOfRatings);
                mediaItem.NumberOfRatings = numberOfRatings;
                mediaItem.AverageRating = avgRating;
            }
            catch (InstanceNotFoundException e) { /*Do nothing - no avg rating found*/ }

            // Add all the UserInformation to targetUser and return it
            mediaItem.Information = informationList;

            return mediaItem;
        }

        /// <summary>
        /// Helper method used by FindMediaItemRange. Gets a range of media items.
        /// </summary>
        /// <param name="from">Where the range must begin</param>
        /// <param name="to">Where the range must end</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <param name="clientId">The id of the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        private Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> 
            GetMediaItems(int from, int to, string clientToken, int clientId)
        {
            var result = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();

            var typeGroups = _storage.Get<Entity>().
                        Where(item => item.ClientId == clientId).
                        OrderBy(a => a.Id).
                        GroupBy(a => a.TypeId);

            foreach (var typeGroup in typeGroups)
            {
                var mediaItemSearchResult = new MediaItemSearchResultDTO();
                mediaItemSearchResult.NumberOfSearchResults = typeGroup.Count();
                var realGroup = typeGroup.Skip(from).Take(to - from);
                var itemList = realGroup.Select(item => GetMediaItemInformation(item.Id, null, clientToken)).ToList();
                if (typeGroup.Key != null)
                {
                    if (itemList.Count > 0)
                    {
                        mediaItemSearchResult.MediaItemList = itemList;
                        result.Add((MediaItemTypeDTO)typeGroup.Key, mediaItemSearchResult);
                    }
                }
                else
                {
                    throw new InvalidOperationException("MediaItemType was not recognized");
                }
            }
            return result;
        }

        /// <summary>
        /// Helper method used by FindMediaItemRange. Gets a range of media items by type.
        /// </summary>
        /// <param name="from">Where the range must begin</param>
        /// <param name="to">Where the range must end</param>
        /// <param name="mediaType">The type of the media item. E.g. MediaItemType.Book for books</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <param name="clientId">The id of the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        private Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> 
            GetMediaItemsByType(int from, int to, MediaItemTypeDTO mediaType, string clientToken, int clientId)
        {
            var result = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();
            var mediaItems = _storage.Get<Entity>().
                        Where(item => item.TypeId == (int)mediaType && item.ClientId == clientId).
                        OrderBy(a => a.Id);

            var mediaItemSearchResult = new MediaItemSearchResultDTO();
            mediaItemSearchResult.NumberOfSearchResults = mediaItems.Count();

            var realMediaItems = mediaItems.Skip(from).Take(to - from);

            var list = new List<MediaItemDTO>();
            foreach (var mediaItem in realMediaItems)
            {
                list.Add(GetMediaItemInformation(mediaItem.Id, null, clientToken));
            }
            
            mediaItemSearchResult.MediaItemList = list;
            result.Add(mediaType, mediaItemSearchResult);
            return result;
        }

        /// <summary>
        /// Helper method used by FindMediaItemRange. Gets a range of media items matching a search key.
        /// </summary>
        /// <param name="from">Where the range must begin</param>
        /// <param name="to">Where the range must end</param>
        /// <param name="searchKey">The search keyword</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <param name="clientId">The id of the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        private Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> 
            SearchMediaItems(int from, int to, string searchKey, string clientToken, int clientId)
        {
            var result = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();
            var typeGroups = _storage.Get<EntityInfo>().
                        Where(ei => ei.Data.Contains(searchKey) && ei.Entity.ClientId == clientId).
                        GroupBy(ei => ei.Entity.TypeId);

            foreach (var type in typeGroups)
            {
                var mediaItemSearchResult = new MediaItemSearchResultDTO();
                var mediaItemGroup = type.GroupBy(ei => ei.EntityId).OrderBy(group => group.Count());
                mediaItemSearchResult.NumberOfSearchResults = mediaItemGroup.Count();
                var typeRange = mediaItemGroup.Skip(from).Take(to - from);

                var list = new List<MediaItemDTO>();
                foreach (var item in typeRange)
                {
                    list.Add(GetMediaItemInformation(item.Key, null, clientToken));
                }
                if (type.Key != null)
                {
                    if (list.Count > 0)
                    {
                        mediaItemSearchResult.MediaItemList = list;
                        result.Add((MediaItemTypeDTO)type.Key, mediaItemSearchResult);
                    }
                }
                else
                {
                    throw new InvalidOperationException("MediaItemType was not recognized");
                }
            }
            return result;
        }

        /// <summary>
        /// Helper method used by FindMediaItemRange. Gets a range of media items of a specific type
        /// matching a search key.
        /// </summary>
        /// <param name="from">Where the range must begin</param>
        /// <param name="to">Where the range must end</param>
        /// <param name="mediaType">The type of the media item. E.g. MediaItemType.Book for books</param>
        /// <param name="searchKey">The search keyword</param>
        /// <param name="clientToken">The token used to verify the client</param>
        /// <param name="clientId">The id of the client</param>
        /// <returns>A Dictionary where each MediaItemType is a key with a value of a list of MediaItem</returns>
        private Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> SearchMediaItemsByType
            (int from, int to, MediaItemTypeDTO mediaType, string searchKey, string clientToken, int clientId)
        {
            var result = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();
            var mediaItems = _storage.Get<EntityInfo>().
                        Where(info => info.Data.Contains(searchKey)
                                        && info.Entity.TypeId == (int)mediaType
                                        && info.Entity.ClientId == clientId).
                        GroupBy(info => info.EntityId).
                        OrderBy(group => group.Count());

            var mediaItemSearchResult = new MediaItemSearchResultDTO();
            mediaItemSearchResult.NumberOfSearchResults = mediaItems.Count();

            var mediaItemRange = mediaItems.Skip(from).Take(to - from);

            var list = new List<MediaItemDTO>();
            foreach (var mediaItem in mediaItemRange)
            {
                list.Add(GetMediaItemInformation(mediaItem.Key, null, clientToken));
            }

            mediaItemSearchResult.MediaItemList = list;
            result.Add(mediaType, mediaItemSearchResult);
            return result;
        }

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
        /// <exception cref="ArgumentNullException">Thrown when the db context is null</exception>
        public Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> FindMediaItemRange(int from, int to, MediaItemTypeDTO? mediaType, string searchKey, string clientToken)
        {
            Contract.Requires<ArgumentException>(from > 0);
            Contract.Requires<ArgumentException>(to > 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            const int rangeCap = 100;
            if (from > to) { int temp = from; from = to; to = temp; } //Switch values if from > to

            if(to - from >= rangeCap) {throw new ArgumentException("The requested range exceeds the cap of " + rangeCap);}

            from--; //FindMEdiaItemRange(1,3,....) must find top 3. This means Skip(0).Take(3)

            int clientId = _authLogic.CheckClientToken(clientToken);
            if (clientId == -1)
            {
                throw new InvalidClientException("Invalid client token");
            }

            var result = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();

            bool isAllMediaTypes = mediaType.Equals(null);
            bool isSearchKeyNullOrEmpty = string.IsNullOrEmpty(searchKey);

            if (isAllMediaTypes)
            {
                result = isSearchKeyNullOrEmpty //If no searchKey: GetMediaItems. Else SearchMediaItems
                    ? GetMediaItems(from, to, clientToken, clientId) 
                    : SearchMediaItems(from, to, searchKey, clientToken, clientId);
            }
            else
            {
                result = isSearchKeyNullOrEmpty //If no searchKey: GetMediaItemsByType. Else SearchMediaItemsByType
                    ? GetMediaItemsByType(from, to, (MediaItemTypeDTO) mediaType, clientToken, clientId)
                    : SearchMediaItemsByType(from, to, (MediaItemTypeDTO) mediaType, searchKey, clientToken, clientId);
            }
            return result;
        }

        /// <summary>
        /// Associates a user with a media item and includes a value from 1-10 representing the rating.
        /// </summary>
        /// <param name="user">The user who wishes to rate a media item</param>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="rating">The rating from 1-10</param>
        /// <param name="clientToken">A token used to verify the client</param>
        public void RateMediaItem(UserDTO user, int mediaItemId, int rating, string clientToken)
        {
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentException>(0 < rating && rating <= 10);
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            //check if client has access
            int clientId = _authLogic.CheckClientToken(clientToken);
            if (clientId == -1)
            {
                throw new InvalidClientException("Invalid client token");
            }

            //check if the user exists
            int userId = _authLogic.CheckUserExists(user);
            if (userId == -1)
            {
                throw new InvalidUserException();
            }

            //check if the user has already rated this media item
            var existing = _storage.Get<Rating>().Where(a => a.UserId == userId && a.EntityId == mediaItemId).Select(a => a).FirstOrDefault();
            if (existing != null)
            {
                //Update
                existing.Value = rating;
                _storage.Update(existing);
            }
            else
            {
                var validMediaItem = _storage.Get<Entity>().Any(a => a.Id == mediaItemId);
                if (validMediaItem)
                {
                    var newRating = new Rating
                    {
                        UserId = userId,
                        EntityId = mediaItemId,
                        Value = rating
                    };
                    _storage.Add(newRating);
                }
                else
                {
                    throw new MediaItemNotFoundException("Media item with id " + mediaItemId + "not found");
                }
            }
        }

        /// <summary>
        /// Gets the average rating of a media item
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="numberOfRatings">The number of ratings will be returned here</param>
        /// <returns>A double representing the average rating</returns>
        /// <exception cref="ArgumentException">Thrown when the mediaItemId is less than 1</exception>
        internal double GetAverageRating(int mediaItemId, out int numberOfRatings)
        {
            Contract.Requires<ArgumentException>(mediaItemId > 0);

            if (_storage.Get<Rating>().Any(a => a.EntityId == mediaItemId))
            {
                var ratings = _storage.Get<Rating>().Where(a => a.EntityId == mediaItemId);
                numberOfRatings = ratings.Count();
                return ratings.Average(a => a.Value);
            }

            numberOfRatings = 0;
            return 0.0;
        }

        /// <summary>
        /// Deletes a media item and all of its associations if the user has the right to do so. 
        /// Only admins and owners are allowed to delete media items.
        /// </summary>
        /// <param name="user">The user who wishes to delete a media item</param>
        /// <param name="mediaItemId">The id of the media item to be deleted</param>
        /// <param name="clientToken">A token used to verify the client</param>
        /// <exception cref="ArgumentException">Thrown when the mediaItemId is not > 0</exception>
        /// <exception cref="ArgumentNullException">Thrown when the user or the clientToken is null</exception>
        /// <exception cref="InvalidCredentialException">Thrown when the clientToken is not accepted</exception>
        /// <exception cref="AccessViolationException">Thrown when the requesting user is not allowed to delete the media item</exception>
        /// <exception cref="FaultException&lt;UnauthorizedUser&gt;">Thrown when the user credentials are not accepted</exception>
        public void DeleteMediaItem(UserDTO user, int mediaItemId, string clientToken)
        {
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            //check if client has access
            int clientId = _authLogic.CheckClientToken(clientToken);
            if (clientId == -1)
            {
                throw new InvalidClientException("Invalid client token");
            }

            //check if the user exists
            int userId = _authLogic.CheckUserExists(user);
            if (userId == -1) 
            {
                throw new InvalidUserException();
            }

            var mediaItem = _storage.Get<Entity>().FirstOrDefault(foo => foo.Id == mediaItemId);
            if (mediaItem == null)
            {
                throw new MediaItemNotFoundException("Media item with id " + mediaItemId + " was not found");
            }

            var isUserAdmin = _authLogic.IsUserAdminOnClient(userId, clientToken);
            var userAccessRight = _authLogic.CheckUserAccess(userId, mediaItemId);
            if (isUserAdmin || userAccessRight == AccessRightType.Owner)
            {
                if (File.Exists(mediaItem.FilePath))
                {
                    File.Delete(mediaItem.FilePath);
                }
                //else Do nothing. The file is not there anyway

                //Delete thumbnail
                var thumbnailPath = mediaItem.EntityInfo.
                    Where(a => a.EntityInfoTypeId == (int) InformationTypeDTO.Thumbnail).
                    Select(a => a.Data).
                    FirstOrDefault();
                if (thumbnailPath != null)
                {
                    FileStorage.DeleteThumbnail(mediaItemId, Path.GetExtension(thumbnailPath));
                }
                //else Do nothing. The file has no thumbnail OR the thumbnail does not exist on the path
                _storage.Delete(mediaItem.EntityInfo);
                _storage.Delete(mediaItem.AccessRight);
                _storage.Delete(mediaItem.Rating);
                _storage.Delete<Entity>(mediaItemId);
            }
            else
            {
                throw new UnauthorizedUserException("The user is not allowed to delete this media item");
            }
        }
        /// <summary>
        /// Update an Entity in the database so it holds the information given by a MediaItemDTO.
        /// </summary>
        /// <param name="user">The user who requested the update.</param>
        /// <param name="media">The MediaItemDTO holding the new information.</param>
        /// <param name="clientToken">A token to verify the client.</param>
        /// <exception cref="InvalidClientException">If the clientToken isn't valid.</exception>
        /// <exception cref="InvalidUserException">If the username and password of the user don't match.</exception>
        /// <exception cref="UnauthorizedAccessException">If the user isn't allowed to perform the update.</exception>
        /// <exception cref="MediaItemNotFoundException">If no entity was found with the given id.</exception>
        public void UpdateMediaItem(UserDTO user, MediaItemDTO media, string clientToken)
        {
            Contract.Requires<ArgumentNullException>(user != null);
            Contract.Requires<ArgumentNullException>(media != null);
            Contract.Requires<ArgumentNullException>(clientToken != null);
            Contract.Requires<ArgumentException>(media.Id > 0);
            //Validate clientToken
            if (_authLogic.CheckClientToken(clientToken) == -1)
            {
                throw new InvalidClientException();
            }
            //Validate User credentials.
            user.Id = _authLogic.CheckUserExists(user);
            if (user.Id == -1)
                throw new InvalidUserException();
            //Validate that user is allowed to update.
            if(_authLogic.CheckUserAccess(user.Id, media.Id) != AccessRightType.Owner && !_authLogic.IsUserAdminOnClient(user.Id, clientToken))
                throw new UnauthorizedAccessException();
            //See if there is an item in the db with matching id.
            var entity = _storage.Get<Entity>().FirstOrDefault(x => x.Id == media.Id);

            //only update if there is something to update.
            if (entity == null) throw new MediaItemNotFoundException();
            //Map any MediaItemInformationDTO in the Information collection to an EntityInfo, ignoring any null entries.
            List<EntityInfo> information = media.Information == null
                ? new List<EntityInfo>()
                : media.Information.Aggregate(new List<EntityInfo>(), (list, x) =>
                {
                    if (x != null && x.Data != null) list.Add(new EntityInfo() { Data = x.Data, EntityInfoTypeId = (int)x.Type});
                    return list;
                });
            //Delete old infos.
            _storage.Delete(entity.EntityInfo);
            //Set type and information to be the new values and update db.
            entity.TypeId = (int)media.Type;
            entity.EntityInfo = information;
            _storage.Update(entity);
        }

        public void Dispose()
        {
            _storage.Dispose();
            _authLogic.Dispose();
        }
    }
}
