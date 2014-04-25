using System.Web.Mvc;
using ArtShare.Models;
using ShareItServices.UserService;

namespace ArtShare.Logic
{
    public interface IAccountLogic
    {
        void RegisterAccount(RegisterModel model);
        void DeleteAccount(int id);
        void UpdateAccountInformation(AccountModel model);
    }
}