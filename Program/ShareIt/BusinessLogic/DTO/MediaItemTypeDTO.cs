using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public enum MediaItemTypeDTO
    {
        [EnumMember]Movie=1,
        [EnumMember]Book,
        [EnumMember]Music
    }
}
