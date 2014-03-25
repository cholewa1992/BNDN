using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class ClientDTO
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Token { get; set; }
    }
}