using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer
{
    public interface IDataTransferLogic
    {
        FileStream GetMediaFileStream(Client client, User user, int id);
    }
}
