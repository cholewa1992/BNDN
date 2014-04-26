﻿using System;
using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class AccessRightDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int MediaItemId { get; set; }

        [DataMember]
        public DateTime? Expiration { get; set; }
    }
}
