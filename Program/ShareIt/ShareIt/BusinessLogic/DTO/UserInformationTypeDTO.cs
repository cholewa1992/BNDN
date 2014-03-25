using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BusinessLogicLayer.DTO
{
    public enum UserInformationTypeDTO
    {
        [EnumMember]Firstname=1,
        [EnumMember]Lastname,
        [EnumMember]Email,
        [EnumMember]Location
        
    }
}
