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
            var mii1 = new MediaItemInformation
            {
                Id = 1,
                Type = InformationType.Title,
                Data = "Some stub title"
            };
            var mii2 = new MediaItemInformation
            {
                Id = 2,
                Type = InformationType.Price,
                Data = "10.0"
            };

            var list = new List<MediaItemInformation> {mii1, mii2};

            var item = new MediaItem {Id = 1, Type = MediaItemType.Book, FileExtension = ".pdf", Information = list};

            return item;
        }
    }
}
