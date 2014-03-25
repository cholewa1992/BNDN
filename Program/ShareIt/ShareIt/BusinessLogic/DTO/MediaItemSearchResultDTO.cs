using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class MediaItemSearchResultDTO
    {
        [DataMember] 
        public List<MediaItemDTO> MediaItemList { get; set; }

        [DataMember]
        public int NumberOfSearchResults { get; set; }
    }
}
