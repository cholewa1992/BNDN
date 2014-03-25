using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class UserInformationDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public UserInformationTypeDTO Type { get; set; }

        [DataMember]
        public string Data { get; set; }

    }
}
