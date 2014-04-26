using System.Web.Mvc;
using ArtShare.Models;
using ShareItServices.UserService;

namespace ArtShare.Logic
{
    public interface IAccountLogic
    {
        void RegisterAccount(RegisterModel model);
        void DeleteAccount(int id);

        void UpdateAccountInformation(string username, string password, AccountModel model);
        AccountModel GetAccountInformation(string username, string password, int userId);
    }
}