using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class UserDTO
    {

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public IEnumerable<UserInformationDTO> Information { get; set; }

        public UserDTO()
        {
            Information = new List<UserInformationDTO>();
        }
    }
}