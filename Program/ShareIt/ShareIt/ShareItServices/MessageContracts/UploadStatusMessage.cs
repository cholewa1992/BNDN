using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ShareItServices.MessageContracts
{
    [MessageContract]
    public class UploadStatusMessage
    {
        [MessageBodyMember(Order = 1)]
        public bool UploadSucceeded { get; set; }
    }
}
