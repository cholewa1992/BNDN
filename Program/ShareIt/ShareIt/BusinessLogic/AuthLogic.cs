using System;
using System.Diagnostics.Contracts;
using System.Linq;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    public class AuthLogic: IAuthInternalLogic
    {

        private IStorageBridge _storage;


        public AuthLogic(IStorageBridge storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Checks whether a user has access
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        public AccessRightType CheckUserAccess(int userId, int mediaItemId)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(_storage != null);
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(mediaItemId > 0);

            // TODO use .single instead of .First

            //Find an accessright
            var ar = _storage.Get<AccessRight>().Where(a => a.UserId == userId && a.EntityId == mediaItemId)
                    .Select(a => a).First();

            //Check if it's past expiration
            if (ar.Expiration != null && DateTime.Now >= ar.Expiration)
                    throw new Exception();

            //Pass the accessrighttype to enum
            var art =
                ParseEnum<AccessRightType>(
                    _storage.Get<DataAccessLayer.AccessRightType>().Where(a => a.Id == ar.AccessRightTypeId)
                        .Select(a => a.Name).First());

            return art;

        }



        public int CheckClientToken(string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(_storage != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));

            
            //return client id or -1
            return _storage.Get<DataAccessLayer.Client>()
                    .Where((c) => c.Token == clientToken)
                    .Select(c => c.Id)
                    .FirstOrDefault(i => i == -1);

        }

        public bool IsUserAdminOnClient(int userId, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentNullException>(_storage != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));
            Contract.Requires<ArgumentException>(userId > 0);

            return _storage.Get<ClientAdmin>().Any(ca => ca.UserId == userId && ca.Client.Token == clientToken);
        }

        public bool CheckUserExists(User user)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Password));


            bool result;

            using (var storage = new StorageBridge(new EfStorageConnection<RentIt08Entities>()))
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

            using (var storage = new StorageBridge(new EfStorageConnection<RentIt08Entities>()))
            {
                result = storage.Get<DataAccessLayer.Client>().Any(c => c.Name == client.Name && c.Token == client.Token);
            }

            return result;
        }



        public void Dispose()
        {
            _storage.Dispose();
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}