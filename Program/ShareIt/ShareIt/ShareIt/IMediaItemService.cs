using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;

namespace ShareIt
{
    /// <summary>
    /// This interface shows available actions that can be performed on media items
    /// </summary>
    [ServiceContract]
    public interface IMediaItemService
    {
        /// <summary>
        /// Get a media item including all of its information.
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem</returns>
        [OperationContract]
        MediaItem GetMediaItemInformation(int mediaItemId, string clientToken);
    }
}

