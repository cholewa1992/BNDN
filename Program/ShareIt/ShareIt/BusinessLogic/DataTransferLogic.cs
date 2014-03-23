using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.ServiceModel;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.FaultDataContracts;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    class DataTransferLogic : IDataTransferLogic
    {
        private readonly IStorageBridge _dbStorage;
        private readonly IFileStorage _fileStorage;
        private readonly IAuthInternalLogic _authLogic;
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
        }
        /// <summary>
        /// Get a stream containing the data of a specific media item.
        /// </summary>
        /// <param name="clientToken">The clientToken of the client which requested the data.</param>
        /// <param name="user">The user who requested the data.</param>
        /// <param name="mediaId">The id of the Media whose data is requested.</param>
        /// <param name="fileExtension">A string for holding the file extension of the media file.</param>
        /// <returns>A Stream containing the data of the media item requested.</returns>
        public Stream GetMediaStream(string clientToken, User user, int mediaId, out string fileExtension)
        {
            Stream result;
            ValidateCredentials(clientToken, user);
            if(_authLogic.CheckUserAccess(user.Id, mediaId) == AccessRightType.NoAccess)
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser
                {

                    Message = "User not allowed to download media with id: " + mediaId
                });
            try
            {
                var entity = _dbStorage.Get<Entity>().Single(e => e.Id == mediaId);
                string filePath = entity.FilePath;
                fileExtension = entity.EntityType.Extension;
                result = _fileStorage.ReadFile(filePath);

            }
            catch (InvalidOperationException)
            {
                throw new FaultException<ArgumentFault>(new ArgumentFault
                    {
                        Message = "No Media found with id: " + mediaId
                    });
            }          
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
        public int SaveMedia(string clientToken, User owner, MediaItem media, Stream stream)
        {
            //Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(clientToken));
            //Contract.Requires<ArgumentNullException>(owner != null);
            //Contract.Requires<ArgumentNullException>(media != null);
            //Contract.Requires<ArgumentNullException>(stream != null);
            //Contract.Requires<ArgumentException>(stream.CanRead);
            var clientId = ValidateCredentials(clientToken, owner);
            //if(!_authLogic.UserCanUpload(owner, clientToken))
            //    throw new FaultException<UnauthorizedUser>(new UnauthorizedUser
            //        {
            //            Message = "User not allowed to upload to client."
            //        }
            //        );
            //Create new entity.
            var entity = MapMediaItem(media, owner, clientId);

            //Store entity and get the id the db assigned to it.
            _dbStorage.Add(entity);
            int result = entity.Id;
            //Save the stream as a file using the id it was given.
            try
            {
                var filePath = _fileStorage.SaveFile(stream, owner.Id, result, media.FileExtension);
                entity.FilePath = filePath;
                _dbStorage.Update(entity);
            }
            //If something went wrong while saving the file, delete the entity from the db and return -1.
            catch (IOException)
            {
                result = -1;
                _dbStorage.Delete(entity);
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
        /// Map a data from a MediaItem into an Entity and set a User as having Owner AccessRight
        /// </summary>
        /// <param name="item">The MediaItem holding the information to map.</param>
        /// <param name="owner">The User who should be set as Owner of the Entity.</param>
        /// <param name="clientId"></param>
        /// <returns>An Entity holding the information given by the MediaItem and User</returns>
        private Entity MapMediaItem(MediaItem item, User owner, int clientId)
        {
            var result = new Entity
            {
                EntityType = new EntityType
                {
                    Extension = item.FileExtension,
                    Id = (int)item.Type
                },
                ClientId = clientId
            };
            result.AcessRight.Add(new AcessRight
            {
                //TODO Add access right type.
                UserId = owner.Id
            });
            foreach (var info in item.Information)
            {
                result.EntityInfo.Add(MapMediaItemInfo(info));
            }
            return result;
        }
        /// <summary>
        /// Map a MediaItemInformation to an EntityInfo
        /// </summary>
        /// <param name="info">The MediaItemInformation to map.</param>
        /// <returns>An EntityInfo holding the information from the MediaItemInformation.</returns>
        private EntityInfo MapMediaItemInfo(MediaItemInformation info)
        {
            return new EntityInfo
            {
                Data = info.Data,
                EntityInfoTypeId = (int)info.Type
            };
        }
        /// <summary>
        /// Validate credentials of a clientToken and User
        /// </summary>
        /// <param name="clientToken">The clientToken to validate.</param>
        /// <param name="user">The User to validate.</param>
        /// <returns>The id of the client if it was validated.</returns>
        /// <exception cref="FaultException{UnauthorizedClient}">If the clientToken was not accepted.</exception>
        /// <exception cref="FaultException{UnauthorizedUser}">If the User was not accepted.</exception>
        private int ValidateCredentials(string clientToken, User user)
        {
            var result = _authLogic.CheckClientToken(clientToken);
            if (result == -1)
                throw new FaultException<UnauthorizedClient>(new UnauthorizedClient
                {
                    Message = "Client token not accepted."
                });
            if (!_authLogic.CheckUserExists(user))
                throw new FaultException<UnauthorizedUser>(new UnauthorizedUser
                {
                    Message = "User credentials not accepted."
                });
            return result;
        }
    }
}
