using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Linq;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer.Stub
{
    public class MediaItemLogicStub : IMediaItemLogic
    {
        /// <summary>
        ///     Returns a stub media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItem GetMediaItemInformation(int mediaItemId, string clientToken)
        {
            var list = new List<MediaItemInformation> {
                new MediaItemInformation
                {
                    Id = 1,
                    Type = InformationType.Title,
                    Data = "Harry Potter And The Chamber Of Secrets"
                },
                new MediaItemInformation
                {
                    Id = 1,
                    Type = InformationType.Price,
                    Data = "6.64"
                },
                new MediaItemInformation
                {
                    Id = 1,
                    Type = InformationType.NumberOfPages,
                    Data = "341"
                },
                new MediaItemInformation
                {
                    Id = 1,
                    Type = InformationType.Genre,
                    Data = "Fantasy"
                },
                new MediaItemInformation
                {
                    Id = 1,
                    Type = InformationType.Author,
                    Data = "J.K. Rowling"
                }
            };

            var item = new MediaItem {Id = 1, Type = MediaItemType.Book, FileExtension = ".pdf", Information = list};

            return item;
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
                        var groups = storage.Get<Entity>().GroupBy((a) => a.TypeId).Skip(from).Take(to - from);
                        foreach (var group in groups)
                        {
                            var list = new List<MediaItem>();
                            foreach (var item in group)
                            {
                                list.Add(GetMediaItemInformation(item.Id, "token"));
                            }
                            result.Add((MediaItemType)group.Key, list);
                        }
                    }
                    else //Searchkey & all media types
                    {
                        var typeGroups = (storage.Get<EntityInfo>().
                                Where(a => a.Data.Contains(searchKey)).
                                GroupBy(b => b.EntityId)).
                            GroupBy(c => c.FirstOrDefault().Entity.TypeId).
                            Skip(from).Take(to - from);

                        foreach (var type in typeGroups)
                        {
                            var list = new List<MediaItem>();
                            foreach (var item in type)
                            {
                                list.Add(GetMediaItemInformation(item.Key, "token"));
                            }
                            result.Add((MediaItemType)type.Key, list);
                        }
                    }
                }
                else //A specific media type
                {
                    if (string.IsNullOrEmpty(searchKey)) //No searchkey & specific media type
                    {
                        var mediaItems = storage.Get<Entity>().
                            Where(a => a.TypeId == (int)mediaType).
                            Skip(from).
                            Take(to - from);

                        var list = new List<MediaItem>();
                        foreach (var mediaItem in mediaItems)
                        {
                            list.Add(GetMediaItemInformation(mediaItem.Id, "token"));
                        }
                        result.Add((MediaItemType)mediaType, list);
                    }
                    else //Searchkey & specific media type
                    {
                        var mediaItems = storage.Get<EntityInfo>().
                                Where(a => a.Data.Contains(searchKey)
                                && a.Entity.TypeId == (int)mediaType).
                                GroupBy(b => b.EntityId).
                                Skip(from).
                                Take(to - from);

                        var list = new List<MediaItem>();
                        foreach (var mediaItem in mediaItems)
                        {
                            list.Add(GetMediaItemInformation(mediaItem.Key, "token"));
                        }
                        result.Add((MediaItemType)mediaType, list);
                    }
                }
            }
            return result;
        } 
    }
}
