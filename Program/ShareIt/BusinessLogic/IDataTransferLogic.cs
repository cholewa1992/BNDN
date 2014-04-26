using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IDataTransferLogic : IDisposable
    {
        /// <summary>
        /// Get a stream containing the data of a specific media item.
        /// </summary>
        /// <param name="clientToken">The clientToken of the client which requested the data.</param>
        /// <param name="user">The user who requested the data.</param>
        /// <param name="mediaId">The id of the Media whose data is requested.</param>
        /// <param name="fileExtension">A string for holding the file extension of the media file.</param>
        /// <returns>A Stream containing the data of the media item requested.</returns>
        Stream GetMediaStream(string clientToken, UserDTO user, int mediaId, out string fileExtension);

        /// <summary>
        /// Save the data and metadata of a MediaItem.
        /// </summary>
        /// <param name="clientToken">The client token of the client from which the request originates.</param>
        /// <param name="owner">The User who is requesting to save the media.</param>
        /// <param name="media">The MediaItem object containing the metadata.</param>
        /// <param name="stream">The stream of data which is to be saved.</param>
        /// <returns>The Id which the MediaItem has been assigned by the system.s</returns>
        int SaveMedia(string clientToken,UserDTO owner, MediaItemDTO media, Stream stream);

        /// <summary>
        /// Save a thumbnail and associate it with a media.
        /// </summary>
        /// <param name="clientToken">The client token of the client from which the request originates.</param>
        /// <param name="owner">The user who attempts to add the thumbnail to the media.</param>
        /// <param name="mediaId">The id of the media which the thumbnail should be associated with.</param>
        /// <param name="fileExtension">The file extension of the thumbnail</param>
        /// <param name="fileByteStream">The stream which contains the binary data of the thumbnail.</param>
        /// <returns>A string containging the URL where the thumbnail can be accessed.</returns>
        string SaveThumbnail(string clientToken, UserDTO owner, int mediaId,string fileExtension, Stream fileByteStream);
    }
}
