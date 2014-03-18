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

        public bool CheckUserAccess(int userId, int mediaItemId, IStorageBridge storage)
        {
            return storage.Get<AcessRight>().Any((a) => a.EntityId == mediaItemId && a.UserId == userId);
        }

        public bool CheckClientAccess(Client client, MediaItem mediaItem, IStorageBridge storage)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckClientToken(string clientToken)
        {
            throw new NotImplementedException();
        }


        public bool CheckClientToken(string clientToken, IStorageBridge storage)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));

            return storage.Get<DataAccessLayer.Client>().Any((c) => c.Token == clientToken);


        }

        public bool IsUserAdminOnClient(User user, string clientToken, IStorageBridge storage)
        {
            return storage.Get<ClientAdmin>().Any(ca => ca.UserId == user.Id && ca.Client.Token == clientToken);
        }

        public bool CheckUserExists(User user)
        {
            bool result;

            using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                result = storage.Get<UserAcc>().Any(ua => ua.Username == user.Username && ua.Password == user.Password);
            }

            return result;
        }


        public bool CheckUserExists(User user, IStorageBridge storage)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Password));

            return storage.Get<UserAcc>().Any(ua => ua.Username == user.Username && ua.Password == user.Password);
        }
        
        public bool CheckClientExists(Client client)
        {
            bool result;

            using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                result = storage.Get<DataAccessLayer.Client>().Any(c => c.Name == client.Name && c.Token == client.Token);
            }

            return result;
        }

        public bool CheckClientExists(Client client, IStorageBridge storage)
        {
            return storage.Get<DataAccessLayer.Client>().Any(c => c.Name == client.Name && c.Token == client.Token);
        }
    }
}