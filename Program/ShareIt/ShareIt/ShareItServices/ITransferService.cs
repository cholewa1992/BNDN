using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using ShareItServices.MessageContracts;

namespace ShareItServices
{
    [ServiceContract]
    public interface ITransferService
    {
        [OperationContract]
        MediaTransferMessage DownloadMedia(DownloadRequest request);

        [OperationContract]
        void UploadMedia(MediaTransferMessage media);
    }
}
