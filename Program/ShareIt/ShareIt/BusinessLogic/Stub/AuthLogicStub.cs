using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer.Stub
{
    public class AuthLogicStub: IAuthLogic
    {
        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mediaItemId"></param>
        /// <returns></returns>
        public bool CheckUserAccess(int userId, int mediaItemId)
        {
            return true;
        }

        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="client"></param>
        /// <param name="mediaItem"></param>
        /// <returns></returns>
        public bool CheckClientAccess(Client client, MediaItem mediaItem)
        {
            return true;
        }

        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="clientToken"></param>
        /// <returns></returns>
        public bool CheckClientPassword(string clientToken)
        {
            return true;
        }

        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientToken"></param>
        /// <returns></returns>
        public bool IsUserAdminOnClient(User user, string clientToken)
        {
            return true;
        }

        public bool CheckUserAccess(int userId, int mediaItemId, IStorageBridge storage)
        {
            return true;
        }

        public bool CheckClientAccess(Client client, MediaItem mediaItem, IStorageBridge storage)
        {
            return true;
        }

        public bool CheckClientToken(string clientToken)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckClientToken(string clientToken, IStorageBridge storage)
        {
            return true;
        }

        public bool IsUserAdminOnClient(User user, string clientToken, IStorageBridge storage)
        {
            return true;
        }

        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckUserExists(User user)
        {
            return true;
        }

        public bool CheckUserExists(User user, IStorageBridge storage)
        {
            return true;
        }

        public bool CheckClientExists(Client client)
        {
            return true;
        }

        public bool CheckClientExists(Client client, IStorageBridge storage)
        {
            return true;
        }
    }
}