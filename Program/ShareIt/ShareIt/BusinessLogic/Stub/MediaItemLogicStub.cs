using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public List<List<MediaItem>> FindMediaItemRange(int from, int to, string searchKey)
        {
            /*//for now it's hardcoded
            var books = new List<MediaItem>();
            var music = new List<MediaItem>();
            var movies = new List<MediaItem>();
            var lists = new List<List<MediaItem>>() {books, music, movies};*/
           
            /*
             * Figure out how to use the storage. It must be passed as an argument BUT not from the service.
             * Maybe helper methods taking an IStorageBridge should be used.
             * 
            var lists = new List<List<MediaItem>>();
            //foreach media item type add a new list to lists
            foreach (var type in storage.Get<EntityType>())
            {
                lists.Add(new List<MediaItem>());
                Console.WriteLine(type.Type);
            }

            if (string.IsNullOrEmpty(searchKey))
            {
                //No searchkey - return the range with no filter
                //lists[0].Add();
                var groups = storage.Get<Entity>().GroupBy((a) => a.TypeId).Skip(from).Take(to);
                foreach (var group in groups)
                {
                    Console.WriteLine("Items with TypeId " + group.Key);
                    foreach (var item in group)
                    {
                        Console.WriteLine("Item with id " + item.Id + " has the following info:");
                        foreach (var info in item.EntityInfo)
                        {
                            Console.WriteLine(info.Id + " of InfoType" + info.EntityInfoTypeId +": " + info.Data);
                        } 
                    }
                }
            }
            else
            {
                //Filter the range by the searchkey
                
            }*/
            throw new NotImplementedException();
        }
    }
}
