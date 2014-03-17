using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;
using Client = BusinessLogicLayer.DTO.Client;

namespace BusinessLogicLayer
{
    class DataTransferLogic : IDataTransferLogic
    {
        public FileStream GetMediaFileStream(Client client, User user, int id)
        {
            throw new NotImplementedException();
            using (var storage = new StorageBridge(new EfStorageConnection<BNDNEntities>()))
            {
                var storageClient = storage.Get<DataAccessLayer.Client>().SingleOrDefault(c => c.Name == client.Name);
            }
        }
    }
}
