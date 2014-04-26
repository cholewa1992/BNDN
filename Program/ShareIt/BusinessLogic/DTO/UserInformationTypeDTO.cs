using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BusinessLogicLayer.DTO
{
    public enum UserInformationTypeDTO
    {
        [EnumMember]Email = 1,
        [EnumMember]Firstname,
        [EnumMember]Lastname,
        [EnumMember]Location
        
    }
}
