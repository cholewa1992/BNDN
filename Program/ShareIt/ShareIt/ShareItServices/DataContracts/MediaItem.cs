using System.Runtime.Serialization;

namespace ShareItServices.DataContracts
{
    [DataContract]
    public class MediaItem
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public string Extenstion { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}