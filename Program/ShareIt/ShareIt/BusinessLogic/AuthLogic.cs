using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.Remoting.Messaging;
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
            var ar = _storage.Get<AccessRight>().SingleOrDefault(a => a.UserId == userId && a.EntityId == mediaItemId);


            //Check if any result was found
            if (ar == null)
            {
                return AccessRightType.NoAccess;
            }

            //Grant no access if the expiration is overdue
            if (ar.Expiration != null && DateTime.Now >= ar.Expiration)
            {
                return AccessRightType.NoAccess;
            }

            return ParseEnum<AccessRightType>(
                    _storage.Get<DataAccessLayer.AccessRightType>().Where(a => a.Id == ar.AccessRightTypeId)
                        .Select(a => a.Name).First());

            
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

            
            //return client id or 0
            int result = _storage.Get<DataAccessLayer.Client>()
                .Where((c) => c.Token == clientToken)
                .Select(c => c.Id).FirstOrDefault();

            // TODO implement firstordefault instead of below
            if (result == 0)
            {
                result = -1;
            }

            return result;
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
        /// <returns>bool of whether given user exists with the system</returns>
        public int CheckUserExists(UserDTO user)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Password));


            var result = _storage.Get<UserAcc>()
                .Where(ua => ua.Username == user.Username && ua.Password == user.Password)
                .Select(ua => ua.Id).FirstOrDefault();

            if (result == 0)
            {
                result = -1;
            }

            return result;

        }

        /// <summary>
        /// Checks whether a user object containing Username and Password exists with the system,
        /// if a valid clientToken is provided
        /// </summary>
        /// <param name="user">User object to be checked</param>
        /// <param name="clientToken">Requesters ClientToken</param>
        /// <returns>bool of whether given user exists</returns>
        public int CheckUserExists(UserDTO user, string clientToken)
        {
            if (CheckClientToken(clientToken) == -1)
                throw new UnauthorizedAccessException("Invalid ClientToken");

            return CheckUserExists(user);
        }


        public bool IsUserAdminOnClient(UserDTO user, string clientToken)
        {
            return
                _storage.Get<ClientAdmin>()
                    .Any(ca => ca.Client.Token == clientToken && ca.UserAcc.Username == user.Username);
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

            return _storage.Get<DataAccessLayer.Client>().Any(c => c.Name == client.Name && c.Token == client.Token);
            
        }

        /// <summary>
        /// Gets an expiration date telling when the user who has bought the media item
        /// no longer has access to said media item.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <returns>A DateTime telling when the access expires</returns>
        /// <exception cref="InstanceNotFoundException">Thrown when the access right has already expired 
        /// or when there is no access right and therefore no expiration date</exception>
        public DateTime GetExpirationDate(int userId, int mediaItemId)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(mediaItemId > 0);
            
            //Find the accessright with the latest expiration (if any)
            var ar = _storage.Get<AccessRight>().Where(a => a.UserId == userId && a.EntityId == mediaItemId 
                && a.AccessRightTypeId == (int) AccessRightType.Buyer).
                OrderByDescending(a => a.Expiration).
                Select(a => a).FirstOrDefault();

            if (ar != null)
            {
                if (ar.Expiration == null)
                {
                    return new DateTime(9999, 12, 31);
                } else if (ar.Expiration < DateTime.Now)
                {
                    throw new InstanceNotFoundException("The access right has expired");
                }
                return (DateTime) ar.Expiration;
            }

            throw new InstanceNotFoundException("No expiration date was found");
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