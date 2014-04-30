using System;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    /// <author>
    /// Mathias Pedersen (mkin@itu.dk)
    /// </author>
    internal interface IAuthInternalLogic: IAuthLogic
    {
        AccessRightType CheckUserAccess(int userId, int mediaItemId);
        
        int CheckClientToken(string clientToken);

        bool IsUserAdminOnClient(int userId, string clientToken);

        DateTime? GetBuyerExpirationDate(int userId, int mediaItemId);

        int CheckUserExists(UserDTO user);

    }
}