using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.AccessControl;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class MediaItemDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public MediaItemTypeDTO Type { get; set; }

        [DataMember]
        public string FileExtension { get; set; }

        [DataMember]
        public IEnumerable<MediaItemInformationDTO> Information { get; set; }
        
        [DataMember(IsRequired = false)]
        public UserDTO Owner { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime? ExpirationDate { get; set; }

        [DataMember(IsRequired = false)]
        public double AverageRating { get; set; }

        [DataMember(IsRequired = false)]
        public int NumberOfRatings { get; set; }

        public MediaItemDTO()
        {
            Information = new List<MediaItemInformationDTO>();
        }
    }
}