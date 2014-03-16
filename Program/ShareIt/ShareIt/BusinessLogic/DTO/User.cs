using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}