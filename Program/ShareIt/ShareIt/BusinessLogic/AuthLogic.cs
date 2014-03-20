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

        public IStorageBridge _storage;


        public AuthLogic(IStorageBridge storage)
        {
            _storage = storage;
        }


        public bool CheckUserAccess(int userId, int mediaItemId)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(_storage != null);
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(mediaItemId > 0);

            
            return _storage.Get<AcessRight>().Any((a) => a.EntityId == mediaItemId && a.UserId == userId);
            
        }

        public bool CheckClientAccess(Client client, MediaItem mediaItem)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(_storage != null);

            throw new System.NotImplementedException();
        }


        public bool CheckClientToken(string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(_storage != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));

            return _storage.Get<DataAccessLayer.Client>().Any((c) => c.Token == clientToken);


        }

        public bool IsUserAdminOnClient(User user, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(_storage != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));
            Contract.Requires<ArgumentException>(user.Id > 0);

            return _storage.Get<ClientAdmin>().Any(ca => ca.UserId == user.Id && ca.Client.Token == clientToken);
        }

        public bool CheckUserExists(User user)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Password));


            bool result;

            using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                result = storage.Get<UserAcc>().Any(ua => ua.Username == user.Username && ua.Password == user.Password);
            }

            return result;
        }


        public bool CheckClientExists(Client client)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(client.Name));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(client.Token));


            bool result;

            using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                result = storage.Get<DataAccessLayer.Client>().Any(c => c.Name == client.Name && c.Token == client.Token);
            }

            return result;
        }
    }
}