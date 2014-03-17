using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer.DTO;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUserService" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {
        /// <summary>
        /// Creates an account
        /// </summary>
        /// <param name="user">The user to be created</param>
        [OperationContract]
        void CreateAccount(User user);

        /// <summary>
        /// Returns account information
        /// </summary>
        /// <param name="id">The id of the user of which you want to fetch account information</param>
        /// <returns></returns>
        [OperationContract]
        User GetAccountInformation(int id);

        /// <summary>
        /// Update a user account
        /// </summary>
        /// <param name="user">The user to be updated</param>
        [OperationContract]
        void UpdateAccounInformation(User user);
    }

}
