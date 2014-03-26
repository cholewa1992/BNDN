using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public enum InformationTypeDTO
    {
        [EnumMember]Title=1,
        [EnumMember]Description, 
        [EnumMember]Price,
        [EnumMember]Picture,
        [EnumMember]KeywordTag,
        [EnumMember]Genre,
        [EnumMember]TrackLength,
        [EnumMember]Runtime,
        [EnumMember]NumberOfPages,
        [EnumMember]Author,
        [EnumMember]Director,
        [EnumMember]Artist,
        [EnumMember]CastMember,
        [EnumMember]ReleaseDate,
        [EnumMember]Language, 
        [EnumMember]ExpirationDate
    }
}
