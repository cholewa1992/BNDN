using System;
using System.Diagnostics.Contracts;
using System.Linq;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    public class AuthLogic: IAuthLogic
    {

        public bool CheckUserAccess(User user, MediaItem mediaItem, IStorageBridge storage)
        {
            return storage.Get<AcessRight>().Any((a) => a.EntityId == mediaItem.Id && a.UserId == user.Id);
        }

        public bool CheckClientAccess(Client client, MediaItem mediaItem, IStorageBridge storage)
        {
            throw new System.NotImplementedException();
        }


        public bool CheckClientPassword(string clientToken, IStorageBridge storage)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));

            return storage.Get<DataAccessLayer.Client>().Any((c) => c.Token == clientToken);

            return true;
        }

        public bool IsUserAdminOnClient(User user, string clientToken, IStorageBridge storage)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckUserExists(User user)
        {
            throw new NotImplementedException();
        }


        public bool CheckUserExists(User user, IStorageBridge storage)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Password));

            // TODO check with DAL

            return true;
        }
        
        public bool CheckClientExists(Client client)
        {
            using (var db = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                
            }
        }

        public bool CheckClientExists(Client client, IStorageBridge storage)
        {
            throw new NotImplementedException();
        }
    }
}