﻿﻿using System;
using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class AccessRight
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int MediaItemId { get; set; }

        [DataMember]
        public DateTime? Expiration { get; set; }

        [DataMember]
        public AccessRightType AccessRightType { get; set; }
    }
}
