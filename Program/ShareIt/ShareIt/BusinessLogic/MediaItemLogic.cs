﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
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
            /*//Preconditions
            Contract.Requires<ArgumentException>(!(mediaItemId < 1));
            Contract.Requires<ArgumentNullException>(clientToken != null);*/
            if(mediaItemId < 1) { throw new ArgumentException(); }
            if(string.IsNullOrEmpty(clientToken)) { throw new ArgumentException(); } 

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
                throw new ArgumentException("No media item with id " + mediaItemId + " exists in the database");
            }

            var mediaItem = new MediaItemDTO { Id = entity.Id, Information = new List<MediaItemInformationDTO>() };
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
                    informationList.Add(new MediaItemInformationDTO
                    {
                        Type = InformationTypeDTO.ExpirationDate,
                        Data = _authLogic.GetExpirationDate((int) userId, mediaItem.Id).ToString(CultureInfo.CurrentCulture)
                    });
                }
                catch (InstanceNotFoundException e)
                {
                    //No expiration date found. Don't do anything else than NOT adding the information
                }
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
            if(from < 1 || to < 1) { throw new ArgumentException("\"from\" and \"to\" must be >= 1");}
            //Contract.Requires<ArgumentException>(!(from < 1 || to < 1));

            const int rangeCap = 100;
            if (from > to) { int temp = from; from = to; to = temp; } //Switch values if from > to

            if(to - from >= rangeCap) {throw new ArgumentException("The requested range exceeds the cap of " + rangeCap);}
            //Contract.Requires<ArgumentException>(to - from < rangeCap);

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
                    var typeGroups = (_storage.Get<EntityInfo>().
                        Where(info => info.Data.Contains(searchKey) && info.Entity.ClientId == clientId).
                        GroupBy(info => info.EntityId).
                        OrderBy(group => group.Count())).
                        GroupBy(d => d.FirstOrDefault().Entity.TypeId);

                    foreach (var type in typeGroups)
                    {
                        var mediaItemSearchResultDTO = new MediaItemSearchResultDTO();
                        mediaItemSearchResultDTO.NumberOfSearchResults = type.Count();
                        var typeRange = type.Skip(from).Take(to - from);

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
                        Where(item => item.TypeId == (int)mediaType && item.ClientId == clientId);

                    var mediaItemSearchResultDTO = new MediaItemSearchResultDTO();
                    mediaItemSearchResultDTO.NumberOfSearchResults = mediaItems.Count();

                    mediaItems = mediaItems.Skip(from).Take(to - from);

                    var list = new List<MediaItemDTO>();
                    foreach (var mediaItem in mediaItems)
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


        public void Dispose()
        {
            _storage.Dispose();
            _authLogic.Dispose();
        }
    }
}
