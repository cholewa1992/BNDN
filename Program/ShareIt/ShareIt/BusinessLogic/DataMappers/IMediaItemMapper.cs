using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer.DataMappers
{
    internal interface IMediaItemMapper
    {
        Entity MapToEntity(MediaItem mediaItem);
    }
}
