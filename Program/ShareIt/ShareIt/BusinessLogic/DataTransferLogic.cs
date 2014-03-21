using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    class DataTransferLogic : IDataTransferLogic
    {
        private IStorageBridge _dbStorage;
        private IFileStorage _fileStorage;

        public DataTransferLogic(IFileStorage fileStorage, IStorageBridge dbStorage)
        {
            _fileStorage = fileStorage;
            _dbStorage = dbStorage;
        }

        public Stream GetMediaStream(string clientToken, User user, int id, out string fileExtension)
        {
            throw new NotImplementedException();
            //using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            //{
            //    var storageClient = storage.Get<DataAccessLayer.Client>().SingleOrDefault(c => c.Name == client.Name);
            //}
        }

        public int SaveMedia(string clientToken, User owner, MediaItem media, Stream stream)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(clientToken));
            Contract.Requires<ArgumentNullException>(owner != null);
            Contract.Requires<ArgumentNullException>(media != null);
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(stream.CanRead);
            int result = -1;
            using (_dbStorage)
            {
                var authLogic = new AuthLogic(_dbStorage);
                if (!authLogic.CheckClientToken(clientToken))
                    throw new InvalidCredentialException("Client token not accepted.");
                if(!authLogic.CheckUserExists(owner))
                    throw new InvalidCredentialException("User credentials not correct.");
                if(!authLogic.UserCanUpload(owner, clientToken))
                    throw new UnauthorizedAccessException("User not allowed to upload to client.");




            }
            return result;
        }

        private Entity MapMediaItem(MediaItem item, User owner)
        {
            var result = new Entity
            {
                TypeId = (int) item.Type,

            };
            return result;
        }

        private EntityInfo MapMediaItemInfo(MediaItemInformation info)
        {
            return new EntityInfo
            {
                Data = info.Data,
                EntityInfoTypeId = (int) info.Type
            };
        }
    }
}
