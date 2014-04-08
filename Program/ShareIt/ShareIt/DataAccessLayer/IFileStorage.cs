using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace DataAccessLayer
{
    public interface IFileStorage
    {
        /// <summary>
        /// Save a stream of data into a file.
        /// </summary>
        /// <param name="stream">The Stream containing the data to be saved.</param>
        /// <param name="userId">The Id of the User who wants to save the data.</param>
        /// <param name="mediaId">The Id which the information about the data has in the database.</param>
        /// <param name="fileExtension">The file extension which the data should be saved as.</param>
        /// <returns>A string containing the path where the file was saved.</returns>
        string SaveMedia(Stream stream, int userId, int mediaId, string fileExtension);
        /// <summary>
        /// Return a stream which contains the data of a file at a given path.
        /// </summary>
        /// <param name="filePath">The path where the file is located.</param>
        /// <returns>A Stream which contains the data of the file at the specified path.</returns>
        Stream ReadFile(string filePath);

        /// <summary>
        /// Save a stream as a thumbnail.
        /// </summary>
        /// <param name="fileByteStream">The stream to be saved.</param>
        /// <param name="mediaId">The id of the media which the thumbnail should be associated with.</param>
        /// <param name="fileExtension">The file extension of the thumbnail.</param>
        /// <returns>A string containing the URL where the thumbnail can be found.</returns>
        string SaveThumbnail(Stream fileByteStream, int mediaId, string fileExtension);

        void DeleteThumbnail(int mediaId, string fileExtension);
    }
}