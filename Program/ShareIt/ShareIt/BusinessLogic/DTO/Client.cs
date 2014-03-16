﻿using System.Runtime.Serialization;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}