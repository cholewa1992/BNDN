using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    class MediaItemLogic : IMediaItemLogic
    {
        /// <summary>
        /// Returns a media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItem GetMediaItemInformation(int mediaItemId, string clientToken)
        {
            //check if mediaItemId exists - if true then
                //get the MediaItem
                //get all MediaItemInformation and add these to a list which is added to the MediaItem
            //else 
                //throw new ArgumentException("No media item with id " + mediaItemId + " exists in the database");
            throw new NotImplementedException();
        }

        public Dictionary<MediaItemType, List<MediaItem>> FindMediaItemRange(int from, int to, MediaItemType? mediaType, string searchKey, string clientToken)
        {
            if (from > to)
            {
                int temp = from;
                from = to;
                to = temp;
            }

            var result = new Dictionary<MediaItemType, List<MediaItem>>();

            bool isAllMediaTypes = mediaType.Equals(null);

            using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                if (isAllMediaTypes)
                {
                    if (string.IsNullOrEmpty(searchKey)) //No searchkey & all media types
                    {
                        var groups = storage.Get<Entity>().
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
                            result.Add((MediaItemType) group.Key, list);
                        }
                    }
                    else //Searchkey & all media types
                    {
                        var typeGroups = (storage.Get<EntityInfo>().
                                Where(info => info.Data.Contains(searchKey)).
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
                            result.Add((MediaItemType) type.Key, list);
                        }
                    }
                }
                else //A specific media type
                {
                    if (string.IsNullOrEmpty(searchKey)) //No searchkey & specific media type
                    {
                        var mediaItems = storage.Get<Entity>().
                            Where(item => item.TypeId == (int)mediaType).
                            Skip(from).
                            Take(to-from);

                        var list = new List<MediaItem>();
                        foreach (var mediaItem in mediaItems)
                        {
                            list.Add(GetMediaItemInformation(mediaItem.Id, "token"));
                        }
                        result.Add((MediaItemType) mediaType, list);
                    }
                    else //Searchkey & specific media type
                    {
                        var mediaItems = storage.Get<EntityInfo>().
                                Where(info => info.Data.Contains(searchKey)
                                && info.Entity.TypeId == (int) mediaType).
                                GroupBy(info => info.EntityId).
                                OrderBy(group => group.Count()).
                                Skip(from).
                                Take(to - from);

                        var list = new List<MediaItem>();
                        foreach (var mediaItem in mediaItems)
                        {
                            list.Add(GetMediaItemInformation(mediaItem.Key, "token"));
                        }
                        result.Add((MediaItemType) mediaType, list);
                    }
                }
            }
            return result;
        } 
    }
}
