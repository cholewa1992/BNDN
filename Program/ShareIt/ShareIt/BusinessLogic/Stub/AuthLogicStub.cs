using ShareItServices.DataContracts;

namespace BusinessLogicLayer.Stub
{
    public class AuthLogicStub: IAuthLogic
    {
        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="user"></param>
        /// <param name="mediaItem"></param>
        /// <returns></returns>
        public bool CheckUserAccess(User user, MediaItem mediaItem)
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
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckClientPassword(Client client)
        {
            return true;
        }

        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="user"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsUserAdminOnClient(User user, Client client)
        {
            return true;
        }
    }
}