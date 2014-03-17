using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    class MediaItemLogic : IMediaItemLogic
    {
        /// <summary>
        /// Returns a media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItem GetMediaItemInformation(int mediaItemId)
        {
            //check if mediaItemId exists - if true then
                //get the MediaItem
                //get all MediaItemInformation and add these to a list which is added to the MediaItem
            //else 
                //throw new exception
            throw new NotImplementedException();
        }
    }
}
