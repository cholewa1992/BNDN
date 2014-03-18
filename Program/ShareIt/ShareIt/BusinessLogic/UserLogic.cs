using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    class UserLogic : IUserLogic
    {
        public bool CreateAccount(User user)
        {

            // Check if the user is already stored in the DB

            // Check constraints on username and password
            if (user.Username.Length < 1 || user.Username.Length > 20)
            {
                throw new ArgumentException("Username must consist of between 1 and 20 characters");
            } 
            else if (user.Password.Length < 1 || user.Password.Length > 50)
            {
                throw new ArgumentException("Password must consist of between 1 and 50 characters");
            }
            else if (!Regex.IsMatch(user.Username, "^[a-zA-Z0-9]+$*"))
            {
                throw new ArgumentException("Username must only consist of alphanumerical characters (a-zA-Z0-9)");
            }
            else if (Regex.IsMatch(user.Password, "\\s"))
            {
                throw new ArgumentException("Password must not contain any whitespace characters");
            }

            try
            {
                // Store the user account
            }
            catch (Exception)
            {
                throw new Exception("The account was not created");
            }

            return true;
        }

        public User GetAccountInformation(int id)
        {
            User user;

            try
            {
               user = new User(); // Get the user from the id
            }
            catch (Exception)
            {
                throw new Exception("The requested accountinformation can not be displayed");
            }
            
            return user;
        }

        public bool UpdateAccountInformation(User user)
        {
            // Get the account from db

            User oldAccount;

            oldAccount = user;

            try
            {
                // Store the user account
            }
            catch (Exception)
            {
               throw new Exception("The account was not updated");
            }

            return true;
        }
    }
}
