using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public enum InformationType
    {
        [EnumMember]Title=1,
        [EnumMember]Description, 
        [EnumMember]Price,
        [EnumMember]Genre,
        [EnumMember]Runtime,
        [EnumMember]NumberOfPage,
        [EnumMember]TrackLength,
    }
}
