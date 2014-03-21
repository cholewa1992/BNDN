using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IDataTransferLogic
    {
        /// <summary>
        /// Get a stream containing the data of a specific media item.
        /// </summary>
        /// <param name="clientToken">The clientToken of the client which requested the data.</param>
        /// <param name="user">The user who requested the data.</param>
        /// <param name="mediaId">The id of the Media whose data is requested.</param>
        /// <param name="fileExtension">A string for holding the file extension of the media file.</param>
        /// <returns>A Stream containing the data of the media item requested.</returns>
        Stream GetMediaStream(string clientToken, User user, int mediaId, out string fileExtension);

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="clientToken"></param>
        /// <param name="owner"></param>
        /// <param name="media"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        int SaveMedia(string clientToken,User owner, MediaItem media, Stream stream);
    }
}
