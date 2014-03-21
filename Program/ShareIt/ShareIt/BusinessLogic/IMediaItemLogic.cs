using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public interface IMediaItemLogic
    {
        /// <summary>
        /// Returns a media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        MediaItem GetMediaItemInformation(int mediaItemId, string clientToken);

        List<List<MediaItem>> FindMediaItemRange(int from, int to, MediaItemType? type, string searchKey, string clientToken);
    }
}
