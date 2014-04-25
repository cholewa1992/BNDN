using System;
using ArtShare.Models;

namespace ArtShare.Logic
{
    public class AccountLogicStub: IAccountLogic
    {
        public void RegisterAccount(RegisterModel model)
        {
            if(model.Username == "throw") throw new Exception("error");

        }

        public void DeleteAccount(int id)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAccountInformation(AccountModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}