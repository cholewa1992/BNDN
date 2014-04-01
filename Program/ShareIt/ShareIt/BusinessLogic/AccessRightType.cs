using System.Runtime.Serialization;

namespace BusinessLogicLayer
{
    [DataContract]
    public enum AccessRightType
    {
        NoAccess, 
        [EnumMember]Owner, 
        [EnumMember]Buyer
    }
}