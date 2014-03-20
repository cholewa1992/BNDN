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

        public List<List<MediaItem>> FindMediaItemRange(int from, int to, MediaItemType? mediaType, string searchKey, string clientToken)
        {
            if (from > to)
            {
                int temp = from;
                from = to;
                to = temp;
            }

            var lists = new List<List<MediaItem>>();

            bool isAllMediaTypes = mediaType.Equals(null);

            //Maps the media type to the index in "lists"
            var mediaTypeMapping = new Dictionary<int, int>();

            using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                if (isAllMediaTypes)
                {
                    //foreach media item type add a new list to lists
                    int i = 0;
                    foreach (var type in storage.Get<EntityType>())
                    {
                        mediaTypeMapping.Add(type.Id, i);
                        lists.Add(new List<MediaItem>());
                        i++;
                    }
                }
                else
                {
                    //A specific type. Only one list is needed
                    lists.Add(new List<MediaItem>());
                }

                if (isAllMediaTypes)
                {
                    if (string.IsNullOrEmpty(searchKey)) //No searchkey & all media types
                    {
                        var groups = storage.Get<Entity>().GroupBy((a) => a.TypeId).Skip(from).Take(to);
                        foreach (var group in groups)
                        {
                            foreach (var item in group)
                            {
                                lists[mediaTypeMapping[(int)@group.Key]].Add(
                                    GetMediaItemInformation(item.Id, "token")
                                );
                            }
                        }
                    }
                    else //Searchkey & all media types
                    {
                        var typeGroups = (from itemGroup in
                                              (from e in storage.Get<Entity>()
                                               join ei in storage.Get<EntityInfo>() on e.Id equals ei.EntityId
                                               where ei.Data.Contains(searchKey)
                                               group e by e.Id)
                                          group itemGroup by itemGroup.First().EntityType.Id).
                                          Skip(from).Take(to);

                        foreach (var typeGroup in typeGroups)
                        {
                            foreach (var item in typeGroup)
                            {
                                GetMediaItemInformation(item.Key, "token");
                            }
                        }
                    }
                }
                else //A specific media type
                {
                    if (string.IsNullOrEmpty(searchKey)) //No searchkey & specific media type
                    {
                        var mediaItems = storage.Get<Entity>().
                            Where(a => a.EntityType.Id == (int)mediaType).Skip(from).Take(to);
                        foreach (var mediaItem in mediaItems)
                        {
                            lists[0].Add(
                                GetMediaItemInformation(mediaItem.Id, "token")
                            );
                        }
                    }
                    else //Searchkey & specific media type
                    {
                        var mediaItems = (from e in storage.Get<Entity>()
                                          join ei in storage.Get<EntityInfo>() on e.Id equals ei.EntityId
                                          where ei.Data.Contains(searchKey) && e.EntityType.Id == (int)mediaType
                                          select e).Skip(from).Take(to);

                        foreach (var mediaItem in mediaItems)
                        {
                            lists[0].Add(GetMediaItemInformation(mediaItem.Id, "token"));
                        }
                    }
                }
            }
            return lists;
        } 
    }
}
