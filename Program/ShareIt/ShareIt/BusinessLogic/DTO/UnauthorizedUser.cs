using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    [DataContract]
    public class UnauthorizedUser
    {
        [DataMember]
        public string Message { get; set; }
    }
}
