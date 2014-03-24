using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.AccessControl;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class MediaItem
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public MediaItemType Type { get; set; }

        [DataMember]
        public string FileExtension { get; set; }

        [DataMember]
        public IEnumerable<MediaItemInformation> Information { get; set; }

        public MediaItem()
        {
            Information = new List<MediaItemInformation>();
        }
    }
}