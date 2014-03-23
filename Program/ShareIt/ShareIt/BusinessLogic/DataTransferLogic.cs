using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        /// <summary>
        /// Construct a new DataTransferLogic with a given IFileStorage and IStorageBridge.
        /// </summary>
        /// <param name="fileStorage">The IFileStorage that the DataTransferLogic should use.</param>
        /// <param name="dbStorage">The IStorageBridge that the DataTransferLogic should use.</param>
        internal DataTransferLogic(IFileStorage fileStorage, IStorageBridge dbStorage)
        {
            _fileStorage = fileStorage;
            _dbStorage = dbStorage;
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
            using (_dbStorage)
            {
                //var authLogic = new AuthLogic(_dbStorage);
                //if (!authLogic.CheckClientToken(clientToken))
                //    throw new InvalidCredentialException("Client token not accepted.");
                //if(!authLogic.CheckUserExists(user))
                //    throw new InvalidCredentialException("User credentials not correct.");
                //if(!authLogic.CheckUserAccess(user.Id, mediaId))
                //    throw new UnauthorizedAccessException("User not allowed to download media with id: " + mediaId);
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
            int result;
            using (_dbStorage)
            {
                //var authLogic = new AuthLogic(_dbStorage);
                //if (!authLogic.CheckClientToken(clientToken))
                //    throw new InvalidCredentialException("Client token not accepted.");
                //if(!authLogic.CheckUserExists(owner))
                //    throw new InvalidCredentialException("User credentials not correct.");
                //if(!authLogic.UserCanUpload(owner, clientToken))
                //    throw new UnauthorizedAccessException("User not allowed to upload to client.");
                var entity = MapMediaItem(media, owner);
                foreach (var info in media.Information)
                {
                    entity.EntityInfo.Add(MapMediaItemInfo(info));
                }
                _dbStorage.Add(entity);
                result = entity.Id;
                var filePath = _fileStorage.SaveFile(stream, owner.Id, result, media.FileExtension);
                entity.FilePath = filePath;
                _dbStorage.Update(entity);
            }
            return result;
        }

        private Entity MapMediaItem(MediaItem item, User owner)
        {
            var result = new Entity
            {
                EntityType = new EntityType
                {
                    Extension = item.FileExtension,
                    Id = (int)item.Type
                },
                AcessRight = new Collection<AcessRight>()
                {
                    new AcessRight()
                    {
                        UserId = owner.Id
                    }
                },
                EntityInfo = new Collection<EntityInfo>(),
            };
            return result;
        }

        private EntityInfo MapMediaItemInfo(MediaItemInformation info)
        {
            return new EntityInfo
            {
                Data = info.Data,
                EntityInfoTypeId = (int)info.Type
            };
        }

        public void Dispose()
        {
            if(_dbStorage != null)
                _dbStorage.Dispose();
        }
    }
}
