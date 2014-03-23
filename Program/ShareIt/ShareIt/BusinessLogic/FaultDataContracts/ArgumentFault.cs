using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.FaultDataContracts
{
    [DataContract]
    public class ArgumentFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
