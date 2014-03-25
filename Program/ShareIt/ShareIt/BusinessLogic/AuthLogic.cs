using System;
using System.Diagnostics.Contracts;
using System.Linq;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.ClientDTO;

namespace BusinessLogicLayer
{
    public class AuthLogic: IAuthInternalLogic
    {

        private IStorageBridge _storage;


        internal AuthLogic(IStorageBridge storage)
        {
            Contract.Requires<ArgumentNullException>(storage != null);

            _storage = storage;
        }

        /// <summary>
        /// Checks whether a user has access to a given mediaItem
        /// </summary>
        /// <param name="userId">User to check</param>
        /// <param name="mediaItemId">MediaItem to check</param>
        /// <returns>an Enum representation of the access right</returns>
        public AccessRightType CheckUserAccess(int userId, int mediaItemId)
        {
            //Preconditions
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


        /// <summary>
        /// Checks whether a clienttoken exists with the system
        /// </summary>
        /// <param name="clientToken"></param>
        /// <returns>return client id if it exists or -1</returns>
        public int CheckClientToken(string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));

            
            //return client id or -1
            return _storage.Get<DataAccessLayer.Client>()
                    .Where((c) => c.Token == clientToken)
                    .Select(c => c.Id)
                    .FirstOrDefault(i => i == -1);

        }

        /// <summary>
        /// Checks whether a user is an admin on a given client
        /// </summary>
        /// <param name="userId">User to check</param>
        /// <param name="clientToken">Client to check</param>
        /// <returns>a bool of whether the user is admin on the given client</returns>
        public bool IsUserAdminOnClient(int userId, string clientToken)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));
            Contract.Requires<ArgumentException>(userId > 0);

            return _storage.Get<ClientAdmin>().Any(ca => ca.UserId == userId && ca.Client.Token == clientToken);
        }

        /// <summary>
        /// Checks whether a user object containing Username and Password exists with the system
        /// </summary>
        /// <param name="user">Object checked for Username and Password</param>
        /// <returns>userid if any found, or -1</returns>
        public bool CheckUserExists(UserDTO user)
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

        /// <summary>
        /// Checks whether a client with given Name and Token exists with the system
        /// </summary>
        /// <param name="client">Client with Name and Token to be checked</param>
        /// <returns>a bool of whether the client exists with the system</returns>
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


        [ContractInvariantMethod]
        private void InvariantCheck()
        {
            Contract.Invariant(_storage != null);
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