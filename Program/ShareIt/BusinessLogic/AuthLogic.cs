﻿using System;
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
    internal class AuthLogic: IAuthInternalLogic
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


            if (_storage.Get<AccessRight>().Any(ac => ac.EntityId == mediaItemId && ac.UserId == userId && ac.AccessRightTypeId == (int)AccessRightType.Owner))
            {
                return AccessRightType.Owner;
            }


            try
            {
                var expiration = GetBuyerExpirationDate(userId, mediaItemId);
                if (expiration == null)
                {
                    return AccessRightType.Buyer;
                }
                if (expiration.Value.AddMinutes(15.0) >= DateTime.Now)
                {
                    return AccessRightType.Buyer;
                }
            }
            catch (Exception)
            {
                return AccessRightType.NoAccess;
            }
            
            return AccessRightType.NoAccess;

            
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


        /// <summary>
        /// Checks whether a user is admin on a client
        /// </summary>
        /// <param name="user">User with username to check</param>
        /// <param name="clientToken">client to check</param>
        /// <returns>a bool</returns>
        public bool IsUserAdminOnClient(UserDTO user, string clientToken)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Username));

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
        /// <exception cref="InstanceNotFoundException">Thrown when there is no access right and therefore no expiration date</exception>
        public DateTime? GetBuyerExpirationDate(int userId, int mediaItemId)
        {
            //Preconditions
            Contract.Requires<ArgumentException>(userId > 0);
            Contract.Requires<ArgumentException>(mediaItemId > 0);

            var arNoExpiration = _storage.Get<AccessRight>().
                Any(a => a.UserId == userId && a.EntityId == mediaItemId
                && a.AccessRightTypeId == (int) AccessRightType.Buyer && a.Expiration == null);
            if (arNoExpiration)
            {
                return null;
            }

            //Find the accessright with the latest expiration (if any)
            var ar = _storage.Get<AccessRight>().Where(a => a.UserId == userId && a.EntityId == mediaItemId
                && a.AccessRightTypeId == (int)AccessRightType.Buyer).
                OrderByDescending(a => a.Expiration).
                Select(a => a).FirstOrDefault();

            if (ar != null)
            {
                return ar.Expiration;
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