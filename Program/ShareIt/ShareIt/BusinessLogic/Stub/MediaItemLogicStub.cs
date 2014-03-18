using System;
using System.Collections.Generic;
using BusinessLogicLayer.DTO;

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
    }
}
