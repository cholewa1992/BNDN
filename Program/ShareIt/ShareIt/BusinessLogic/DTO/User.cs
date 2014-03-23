using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class User
    {

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public IEnumerable<UserInformation> Information { get; set; }
    }
}