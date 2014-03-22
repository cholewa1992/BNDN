using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class UserInformation
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public UserInformationType Type { get; set; }

        [DataMember]
        public string Data { get; set; }

    }
}
