using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ShareItServices.MessageContracts;

namespace ShareItServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TransferService" in both code and config file together.
    public class TransferService : ITransferService
    {
        public MediaTransferMessage DownloadMedia(DownloadRequest request)
        {
            throw new NotImplementedException();
        }

        public void UploadMedia(MediaTransferMessage media)
        {
            throw new NotImplementedException();
        }
    }
}
