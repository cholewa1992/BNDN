using System;
using System.Diagnostics.Contracts;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    public class AuthLogic: IAuthLogic
    {
        public bool CheckUserAccess(User user, MediaItem mediaItem)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckClientAccess(Client client, MediaItem mediaItem)
        {
            throw new System.NotImplementedException();
        }


        public bool CheckClientPassword(string clientToken)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(clientToken));

           

            return true;
        }

        public bool IsUserAdminOnClient(User user, string clientToken)
        {
            throw new System.NotImplementedException();
        }


        public bool CheckUserExists(User user)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Username));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(user.Password));

            // TODO check with DAL

            return true;
        }
    }
}