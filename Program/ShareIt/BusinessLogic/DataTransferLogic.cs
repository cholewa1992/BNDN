using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Policy;
using System.ServiceModel;
using BusinessLogicLayer.DataMappers;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    internal class DataTransferLogic : IDataTransferLogic
    {
        private readonly IStorageBridge _dbStorage;
        private readonly IFileStorage _fileStorage;
        private readonly IAuthInternalLogic _authLogic;
        public IMediaItemMapper MediaItemMapper { get; set; }
        /// <summary>
        /// Construct a new DataTransferLogic with a given IFileStorage and IStorageBridge.
        /// </summary>
        /// <param name="fileStorage">The IFileStorage that the DataTransferLogic should use.</param>
        /// <param name="dbStorage">The IStorageBridge that the DataTransferLogic should use.</param>
        /// <param name="authLogic">The IAuthInternalLogic that the DataTransferLogic should use.</param>
        internal DataTransferLogic(IFileStorage fileStorage, IStorageBridge dbStorage, IAuthInternalLogic authLogic)
        {
            _fileStorage = fileStorage;
            _dbStorage = dbStorage;
            _authLogic = authLogic;
            MediaItemMapper = new MediaItemMapper();
        }
        /// <summary>
        /// Get a stream containing the data of a specific media item.
        /// </summary>
        /// <param name="clientToken">The clientToken of the client which requested the data.</param>
        /// <param name="user">The user who requested the data.</param>
        /// <param name="mediaId">The id of the Media whose data is requested.</param>
        /// <param name="fileExtension">A string for holding the file extension of the media file.</param>
        /// <returns>A Stream containing the data of the media item requested.</returns>
        public Stream GetMediaStream(string clientToken, UserDTO user, int mediaId, out string fileExtension)
        {
            if (clientToken == null) throw new ArgumentNullException("clientToken");
            if (user == null) throw new ArgumentNullException("user");

            ValidateClientToken(clientToken);
            user.Id = ValidateUser(user);
            //Check that that user can download
            if(_authLogic.CheckUserAccess(user.Id, mediaId) == AccessRightType.NoAccess && !_authLogic.IsUserAdminOnClient(user.Id, clientToken))
                throw new UnauthorizedUserException();

            var entity = _dbStorage.Get<Entity>().SingleOrDefault(e => e.Id == mediaId);
            if (entity == null)
                throw new MediaItemNotFoundException();

            string filePath = entity.FilePath;
            fileExtension = Path.GetExtension(filePath);
            Stream result = _fileStorage.ReadFile(filePath);
                     
            return result;
        }
        /// <summary>
        /// Save the data and metadata of a MediaItem.
        /// </summary>
        /// <param name="clientToken">The client token of the client from which the request originates.</param>
        /// <param name="owner">The User who is requesting to save the media.</param>
        /// <param name="media">The MediaItem object containing the metadata.</param>
        /// <param name="stream">The stream of data which is to be saved.</param>
        /// <returns>The Id which the MediaItem has been assigned by the system.s</returns>
        public int SaveMedia(string clientToken, UserDTO owner, MediaItemDTO media, Stream stream)
        {
            if (clientToken == null) throw new ArgumentNullException("clientToken");
            if (owner == null) throw new ArgumentNullException("owner");
            if (media == null) throw new ArgumentNullException("media");
            if (stream == null) throw new ArgumentNullException("stream");
            if(!stream.CanRead) throw new ArgumentException("Must be able to read stream.", "stream");
            if(media.FileExtension == null)
                throw new ArgumentException("Must have file extension value.", "media");
            var clientId = ValidateClientToken(clientToken);
            owner.Id = ValidateUser(owner);

            //Create new entity.
            var entity = MediaItemMapper.MapToEntity(media);
            entity.ClientId = clientId;
            entity.FilePath = ""; //Set filepath to empty string because of db constraint.
            entity.AccessRight.Add(new AccessRight
            {
                AccessRightTypeId = (int)AccessRightType.Owner,
                UserId = owner.Id
            });

            //Store entity and get the id the db assigned to it.
            _dbStorage.Add(entity);
            int mediaId = entity.Id;
            //Save the stream as a file using the id it was given.
            try
            {
                var filePath = _fileStorage.SaveMedia(stream, owner.Id, mediaId, media.FileExtension);
                entity.FilePath = filePath;
                _dbStorage.Update(entity);
            }
            //If something went wrong while saving the file, delete the entity from the db and return -1.
            catch (IOException)
            {
                mediaId = -1;
                _dbStorage.Delete(entity);
            }
            return mediaId;
        }

        /// <summary>
        /// Save a thumbnail and associate it with a media.
        /// </summary>
        /// <param name="clientToken">The client token of the client from which the request originates.</param>
        /// <param name="owner">The user who attempts to add the thumbnail to the media.</param>
        /// <param name="mediaId">The id of the media which the thumbnail should be associated with.</param>
        /// <param name="fileExtension">The file extension of the thumbnail</param>
        /// <param name="fileByteStream">The stream which contains the binary data of the thumbnail.</param>
        /// <returns>A string containging the URL where the thumbnail can be accessed.</returns>
        public string SaveThumbnail(string clientToken, UserDTO owner, int mediaId,string fileExtension, Stream fileByteStream)
        {
            Contract.Requires<ArgumentNullException>(clientToken != null);
            Contract.Requires<ArgumentNullException>(owner != null);
            Contract.Requires<ArgumentException>(mediaId > 0);
            Contract.Requires<ArgumentNullException>(fileByteStream != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(owner.Password));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(owner.Username));

            ValidateClientToken(clientToken);
            int userId = ValidateUser(owner);
            //Check media exists with given media id.
            if (_dbStorage.Get<Entity>().SingleOrDefault(x => x.Id == mediaId) == null)
                throw new MediaItemNotFoundException();

            //Check user has owner access to media.
            if(_authLogic.CheckUserAccess(userId, mediaId) != AccessRightType.Owner && !_authLogic.IsUserAdminOnClient(userId, clientToken))
                throw new UnauthorizedUserException();
            
            //Save thumbnail and return result.
            string result;
            try
            {
                result = _fileStorage.SaveThumbnail(fileByteStream, mediaId, fileExtension);
                //check if thumbnail already exists for given media.
                var url =
                    _dbStorage.Get<EntityInfo>()
                        .SingleOrDefault(
                            x => x.EntityInfoTypeId == (int)InformationTypeDTO.Thumbnail && x.EntityId == mediaId);
                //Add thumbnail info to database if none existed.
                if (url == null)
                {
                    var entityInfo = new EntityInfo
                    {
                        Data = result,
                        EntityInfoTypeId = (int)InformationTypeDTO.Thumbnail,
                        EntityId = mediaId
                    };
                    _dbStorage.Add(entityInfo);
                }

            }
            catch (IOException)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Dispose the IStorageBridge and IInternalAuthLogic which this DataTransferLogic uses.
        /// </summary>
        public void Dispose()
        {
            if (_dbStorage != null)
                _dbStorage.Dispose();
            if (_authLogic != null)
                _authLogic.Dispose();
        }

        /// <summary>
        /// Validate credentials of a clientToken
        /// </summary>
        /// <param name="clientToken">The clientToken to validate.</param>
        /// <returns>The id of the client if it was validated.</returns>
        /// <exception cref="FaultException{UnauthorizedClient}">If the clientToken was not accepted.</exception>
        private int ValidateClientToken(string clientToken)
        {
            var result = _authLogic.CheckClientToken(clientToken);
            if (result == -1)
                throw new InvalidClientException();
            return result;
        }
        /// <summary>
        /// Validate credentials of a user.
        /// </summary>
        /// <param name="user">The user whose credentials are to be validated.</param>
        /// <returns>The id of the user if his credentials are validated.</returns>
        /// <exception cref="FaultException{UnauthorizedUser}">If the user's credentials aren't validated.</exception>
        private int ValidateUser(UserDTO user)
        {
            var result = _authLogic.CheckUserExists(user);

            if (result == -1)
                throw new InvalidUserException();

            return result;
        }
    }
}
