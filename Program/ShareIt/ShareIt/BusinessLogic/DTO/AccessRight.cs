﻿using System;
using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class AccessRight
    {
        [DataMember]
        public User User { get; set; }

        [DataMember]
        public MediaItem MediaItem { get; set; }

        [DataMember]
        public DateTime Expiration { get; set; }

        [DataMember]
        public string AccessRightType { get; set; }
    }
}
