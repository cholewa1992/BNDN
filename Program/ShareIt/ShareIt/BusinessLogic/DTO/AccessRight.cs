﻿using System;
using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class AccessRight
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int MediaItemId { get; set; }

        [DataMember]
        public DateTime Expiration { get; set; }

        [DataMember]
        public string AccessRightType { get; set; }
    }
}
