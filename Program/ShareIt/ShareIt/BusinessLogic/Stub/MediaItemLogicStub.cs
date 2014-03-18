using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.Stub
{
    public class MediaItemLogicStub : IMediaItemLogic
    {
        /// <summary>
        /// Returns a stub media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItem GetMediaItemInformation(int mediaItemId)
        {
            MediaItemInformation mii1 = new MediaItemInformation{ Id = 1, 
                Type = InformationType.Title, Data = "Some stub title"};
            MediaItemInformation mii2 = new MediaItemInformation{ Id = 2, 
                Type = InformationType.Price, Data = "0"
            };

            List<MediaItemInformation> list = new List<MediaItemInformation> {mii1, mii2};

            MediaItem item = new MediaItem {Id = 1, Type = MediaItemType.Book, FileExtension = ".pdf", Information = list};

            return item;
        }
    }
}
