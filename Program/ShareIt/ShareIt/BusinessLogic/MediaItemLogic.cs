using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    class MediaItemLogic : IMediaItemInternalLogic
    {
        private IStorageBridge _storage;
        private IAuthInternalLogic _authLogic;

        internal MediaItemLogic(IStorageBridge storage, IAuthInternalLogic authLogic)
        {
            _storage = storage;
            _authLogic = authLogic;
        }

        /// <summary>
        /// Returns a media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="userId"></param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItemDTO GetMediaItemInformation(int mediaItemId, int? userId, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            if (_authLogic.CheckClientToken(clientToken) == -1)
            {
                throw new InvalidCredentialException();
            }

            Entity entity;

            try
            {
                entity = (from m in _storage.Get<Entity>() where m.Id == mediaItemId select m).First();
            }
            catch (Exception e)
            {
                throw new FaultException<MediaItemNotFound>(new MediaItemNotFound{
                    Message = "No media item with id " + mediaItemId + " exists in the database"});
            }

            var mediaItem = new MediaItemDTO { Id = entity.Id, 
                                               Information = new List<MediaItemInformationDTO>(),
                                               Type = (MediaItemTypeDTO) entity.TypeId,
                                               FileExtension = Path.GetExtension(entity.FilePath)
            };
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

            if(userId != null) {
                try
                {
                    DateTime? date = _authLogic.GetBuyerExpirationDate((int) userId, mediaItem.Id);
                    if (date == null)
                    {
                        informationList.Add(new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.ExpirationDate,
                            Data = null
                        });
                    }
                    else
                    {
                        informationList.Add(new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.ExpirationDate,
                            Data = date.ToString()
                        });
                    }
                }
                catch (InstanceNotFoundException e)
                {
                    //No expiration date found. Don't do anything else than NOT adding the information
                }
            }

            try
            {
                var avgRating = GetAverageRating(mediaItemId);
                informationList.Add(new MediaItemInformationDTO
                {
                    Type = InformationTypeDTO.AverageRating,
                    Data = avgRating.ToString("#0.00")
                });
            }
            catch (InstanceNotFoundException e)
            {
                //Do nothing - no avg rating found
            }

            // Add all the UserInformation to targetUser and return it
            mediaItem.Information = informationList;

            return mediaItem;
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
                throw new InvalidCredentialException();
            }

            var result = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();

            bool isAllMediaTypes = mediaType.Equals(null);
            bool isSearchKeyNullOrEmpty = string.IsNullOrEmpty(searchKey);

            if (isAllMediaTypes)
            {
                #region No searchkey & all media types
                if (isSearchKeyNullOrEmpty) //No searchkey & all media types
                {
                    var groups = _storage.Get<Entity>().
                        Where(item => item.ClientId == clientId).
                        OrderBy(a => a.Id).
                        GroupBy((a) => a.TypeId);
                    
                    foreach (var group in groups)
                    {
                        var mediaItemSearchResultDTO = new MediaItemSearchResultDTO();
                        mediaItemSearchResultDTO.NumberOfSearchResults = group.Count();
                        var realGroup = group.Skip(from).Take(to - from);
                        var list = new List<MediaItemDTO>();
                        foreach (var item in realGroup)
                        {
                            list.Add(GetMediaItemInformation(item.Id, null, clientToken));
                        }
                        if (@group.Key != null)
                        {
                            if (list.Count > 0)
                            {
                                mediaItemSearchResultDTO.MediaItemList = list;
                                result.Add((MediaItemTypeDTO)@group.Key, mediaItemSearchResultDTO);
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("MediaItemType was not recognized");
                        }
                    }
                }
                #endregion

                #region Searchkey & all media types
                else //Searchkey & all media types
                {
                    var typeGroups = _storage.Get<EntityInfo>().
                        Where(ei => ei.Data.Contains(searchKey) && ei.Entity.ClientId == clientId).
                        GroupBy(ei => ei.Entity.TypeId);
                    
                    foreach (var type in typeGroups)
                    {
                        var mediaItemSearchResultDTO = new MediaItemSearchResultDTO();
                        var mediaItemGroup = type.GroupBy(ei => ei.EntityId).OrderBy(group => group.Count());
                        mediaItemSearchResultDTO.NumberOfSearchResults = mediaItemGroup.Count();
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
                                mediaItemSearchResultDTO.MediaItemList = list;
                                result.Add((MediaItemTypeDTO)type.Key, mediaItemSearchResultDTO);
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("MediaItemType was not recognized");
                        }
                    }
                }
            }
                #endregion

            else //A specific media type
            {
                #region A specific media type
                if (isSearchKeyNullOrEmpty) //No searchkey & specific media type
                {
                    var mediaItems = _storage.Get<Entity>().
                        Where(item => item.TypeId == (int)mediaType && item.ClientId == clientId).
                        OrderBy(a => a.Id);

                    var mediaItemSearchResultDTO = new MediaItemSearchResultDTO();
                    mediaItemSearchResultDTO.NumberOfSearchResults = mediaItems.Count();

                    var realMediaItems = mediaItems.Skip(from).Take(to - from);

                    var list = new List<MediaItemDTO>();
                    foreach (var mediaItem in realMediaItems)
                    {
                        list.Add(GetMediaItemInformation(mediaItem.Id, null, clientToken));
                    }
                    if (mediaType != null)
                    {
                        mediaItemSearchResultDTO.MediaItemList = list;
                        result.Add((MediaItemTypeDTO)mediaType, mediaItemSearchResultDTO);
                    }
                    else
                    {
                        throw new InvalidOperationException("MediaItemType was not recognized");
                    }
                }
                #endregion

                #region Searchkey & specific media type
                else //Searchkey & specific media type
                {
                    var mediaItems = _storage.Get<EntityInfo>().
                        Where(info => info.Data.Contains(searchKey)
                                        && info.Entity.TypeId == (int)mediaType
                                        && info.Entity.ClientId == clientId).
                        GroupBy(info => info.EntityId).
                        OrderBy(group => group.Count());

                    var mediaItemSearchResultDTO = new MediaItemSearchResultDTO();
                    mediaItemSearchResultDTO.NumberOfSearchResults = mediaItems.Count();

                    var mediaItemRange = mediaItems.Skip(from).Take(to - from);

                    var list = new List<MediaItemDTO>();
                    foreach (var mediaItem in mediaItemRange)
                    {
                        list.Add(GetMediaItemInformation(mediaItem.Key, null, clientToken));
                    }
                    if (mediaType != null)
                    {
                        mediaItemSearchResultDTO.MediaItemList = list;
                        result.Add((MediaItemTypeDTO)mediaType, mediaItemSearchResultDTO);
                    }
                    else
                    {
                        throw new InvalidOperationException("MediaItemType was not recognized");
                    }
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// Associates a user with a media item and includes a value from 1-10 representing the rating.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="rating">The rating from 1-10</param>
        /// <param name="clientToken">A token used to verify the client</param>
        public void RateMediaItem(int userId, int mediaItemId, int rating, string clientToken)
        {
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentException>(0 < rating && rating <= 10);
            Contract.Requires<ArgumentNullException>(clientToken != null);

            //check if client has access
            int clientId = _authLogic.CheckClientToken(clientToken);
            if (clientId == -1)
            {
                throw new InvalidCredentialException();
            }

            //check if the user has already rated this media item
            var existing = _storage.Get<Rating>().Where(a => a.UserId == userId && a.EntityId == mediaItemId).Select(a => a).FirstOrDefault();
            if (existing != null)
            {
                //Update
                var updateRating = new Rating
                {
                    Id = existing.Id,
                    UserId = existing.UserId,
                    EntityId = existing.EntityId,
                    Value = rating
                };
                _storage.Update(updateRating);
            }
            else
            {
                var validUser = _storage.Get<UserAcc>().Any(a => a.Id == userId);
                var validMediaItem = _storage.Get<Entity>().Any(a => a.Id == mediaItemId);
                if (validUser && validMediaItem)
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
                    throw new InstanceNotFoundException("Valid user id: " + validUser + ". Valid media item id: " + validMediaItem);
                }

            }

        }

        /// <summary>
        /// Gets the average rating of a media item
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <returns>A double representing the average rating</returns>
        /// <exception cref="InstanceNotFoundException">Thrown when the media item has not been rated</exception>
        internal double GetAverageRating(int mediaItemId)
        {
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            Contract.Requires<ArgumentException>(mediaItemId < int.MaxValue);

            if (_storage.Get<Rating>().Any(a => a.EntityId == mediaItemId))
            {
                return _storage.Get<Rating>().Where(a => a.EntityId == mediaItemId).Average(a => a.Value);
            }
            
            throw new InstanceNotFoundException("Media item with id " + mediaItemId + "has not been rated");
        }

        public void Dispose()
        {
            _storage.Dispose();
            _authLogic.Dispose();
        }
    }
}
