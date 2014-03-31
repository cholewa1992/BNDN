using System;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    internal interface IAuthInternalLogic: IAuthLogic
    {
        AccessRightType CheckUserAccess(int userId, int mediaItemId);
        
        int CheckClientToken(string clientToken);

        bool IsUserAdminOnClient(int userId, string clientToken);

        DateTime GetExpirationDate(int userId, int mediaItemId);

        int CheckUserExists(UserDTO user);

    }
}