using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ShareItServices.DataContracts;

namespace ShareItServices.MessageContracts
{
    [MessageContract]
    public class DownloadRequest
    {
        public User User { get; set; }
        public int MediaId { get; set; }
    }
}
