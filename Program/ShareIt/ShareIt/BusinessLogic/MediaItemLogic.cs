using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public class MediaItemLogic : IMediaItemLogic
    {
        private IStorageBridge _storage;
        private readonly IBusinessLogicFactory _factory;

        public MediaItemLogic(IStorageBridge storage)
        {
            _storage = storage;
            _factory = BusinessLogicFacade.GetTestFactory();
            //_factory = BusinessLogicFacade.GetBusinessFactory();
        }

        /// <summary>
        /// Returns a media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItem GetMediaItemInformation(int mediaItemId, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(mediaItemId < 1);
            Contract.Requires<ArgumentNullException>(clientToken != null);
            AuthLogic authLogic = new AuthLogic(_storage);
            if (authLogic.CheckClientToken(clientToken) == -1)
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

            var mediaItem = new MediaItem { Information = new List<MediaItemInformation>() };
            var informationList = new List<MediaItemInformation>();

            // Add UserInformation to the temporary list object
            foreach (var e in entity.EntityInfo)
            {

                informationList.Add(new MediaItemInformation()
                {
                    Type = (InformationType) e.EntityInfoTypeId,
                    Data = e.Data
                });
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
        public Dictionary<MediaItemType, List<MediaItem>> FindMediaItemRange(int from, int to, MediaItemType? mediaType, string searchKey, string clientToken)
        {
            if (from > to) { int temp = from; from = to; to = temp; } //Switch values if from > to
            from--; //FindMEdiaItemRange(1,3,....) must find top 3. This means Skip(0).Take(3)
            const int rangeCap = 100;

            Contract.Requires<ArgumentException>(from < 1 || to < 1);
            Contract.Requires<ArgumentException>(to - from >= rangeCap);

            var authLogic = new AuthLogic(_storage);
            int clientId = authLogic.CheckClientToken(clientToken);
            if (clientId == -1)
            {
                throw new InvalidCredentialException();
            }
            
            var result = new Dictionary<MediaItemType, List<MediaItem>>();

            bool isAllMediaTypes = mediaType.Equals(null);

            using (_storage)
            {
                if (isAllMediaTypes)
                {
                    if (string.IsNullOrEmpty(searchKey)) //No searchkey & all media types
                    {
                        var groups = _storage.Get<Entity>().
                            Where(item => item.ClientId == clientId).
                            GroupBy((a) => a.TypeId).
                            Skip(from).
                            Take(to-from);
                        foreach (var group in groups)
                        {
                            var list = new List<MediaItem>();
                            foreach (var item in group)
                            {
                                list.Add(GetMediaItemInformation(item.Id, "token"));
                            }
                            if (@group.Key != null)
                            {
                                result.Add((MediaItemType) @group.Key, list);
                            }
                            else
                            {
                                throw new InvalidOperationException("MediaItemType was not recognized");
                            }
                        }
                    }
                    else //Searchkey & all media types
                    {
                        var typeGroups = (_storage.Get<EntityInfo>().
                                Where(info => info.Data.Contains(searchKey) && info.Entity.ClientId == clientId).
                                GroupBy(info => info.EntityId).
                                OrderBy(group => group.Count())).
                            GroupBy(d => d.FirstOrDefault().Entity.TypeId).
                            Skip(from).Take(to - from);

                        foreach (var type in typeGroups)
                        {
                            var list = new List<MediaItem>();
                            foreach (var item in type)
                            {
                                list.Add(GetMediaItemInformation(item.Key, "token"));
                            }
                            if (type.Key != null)
                            {
                                result.Add((MediaItemType) type.Key, list);
                            }
                            else
                            {
                                throw new InvalidOperationException("MediaItemType was not recognized");
                            }
                        }
                    }
                }
                else //A specific media type
                {
                    if (string.IsNullOrEmpty(searchKey)) //No searchkey & specific media type
                    {
                        var mediaItems = _storage.Get<Entity>().
                            Where(item => item.TypeId == (int)mediaType && item.ClientId == clientId).
                            Skip(from).
                            Take(to-from);

                        var list = new List<MediaItem>();
                        foreach (var mediaItem in mediaItems)
                        {
                            list.Add(GetMediaItemInformation(mediaItem.Id, "token"));
                        }
                        if (mediaType != null)
                        {
                            result.Add((MediaItemType) mediaType, list);
                        }
                        else
                        {
                            throw new InvalidOperationException("MediaItemType was not recognized");
                        }
                    }
                    else //Searchkey & specific media type
                    {
                        var mediaItems = _storage.Get<EntityInfo>().
                                Where(info => info.Data.Contains(searchKey)
                                && info.Entity.TypeId == (int) mediaType
                                && info.Entity.ClientId == clientId).
                                GroupBy(info => info.EntityId).
                                OrderBy(group => group.Count()).
                                Skip(from).
                                Take(to - from);

                        var list = new List<MediaItem>();
                        foreach (var mediaItem in mediaItems)
                        {
                            list.Add(GetMediaItemInformation(mediaItem.Key, "token"));
                        }
                        if (mediaType != null)
                        {
                            result.Add((MediaItemType) mediaType, list);
                        }
                        else
                        {
                            throw new InvalidOperationException("MediaItemType was not recognized");
                        }
                    }
                }
            }
            return result;
        } 
    }
}
