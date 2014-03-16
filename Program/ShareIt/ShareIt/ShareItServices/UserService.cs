using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.DTO;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in both code and config file together.
    public class UserService : IUserService
    {
        private IBusinessLogicFactory _factory;

        /// <summary>
        /// Construct a TransferService which uses the default business logic factory.
        /// This constructor is called by WCF.
        /// </summary>
        public UserService()
        {
            _factory = BusinessLogicFacade.GetBusinessFactory();
        }

        /// <summary>
        /// Construct a TransferService object which uses a specified IBusinessLogicFactory.
        /// Should be used for test purposes.
        /// </summary>
        /// <param name="factory">The IBusinessLogicFactory which the TransferService should use for its logic.</param>
        public UserService(IBusinessLogicFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Creates an account
        /// </summary>
        /// <param name="user">The user to be created</param>
        public void CreateAccount(User user)
        {
            _factory.CreateUserLogic().CreateAccount(user);
        }

        /// <summary>
        /// Returns account information
        /// </summary>
        /// <param name="id">The id of the user of which you want to fetch account information</param>
        /// <returns></returns>
        public User GetAccountInformation(int id)
        {
            return _factory.CreateUserLogic().GetAccountInformation(id);
        }

        /// <summary>
        /// Update a user account
        /// </summary>
        /// <param name="user">The user to be updated</param>
        public void UpdateAccounInformation(User user)
        {
            _factory.CreateUserLogic().UpdateAccountInformation(user);
        }
    }
}
