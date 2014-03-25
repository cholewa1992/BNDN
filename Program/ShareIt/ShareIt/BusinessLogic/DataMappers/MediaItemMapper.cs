using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer.DataMappers
{
    internal class MediaItemMapper : IMediaItemMapper
    {
        /// <summary>
        /// Map a data from a MediaItem into an Entity
        /// </summary>
        /// <param name="item">The MediaItem holding the information to map.</param>
        /// <returns>An Entity holding the information given by the MediaItem</returns>
        public Entity MapToEntity(MediaItemDTO item)
        {
            var result = new Entity
            {
                TypeId = (int)item.Type,
            };
            foreach (var info in item.Information)
            {
                result.EntityInfo.Add(MapMediaItemInfo(info));
            }
            return result;
        }

        /// <summary>
        /// Map a MediaItemInformation to an EntityInfo
        /// </summary>
        /// <param name="info">The MediaItemInformation to map.</param>
        /// <returns>An EntityInfo holding the information from the MediaItemInformation.</returns>
        private EntityInfo MapMediaItemInfo(MediaItemInformationDTO info)
        {
            return new EntityInfo
            {
                Data = info.Data,
                EntityInfoTypeId = (int)info.Type
            };
        }
    }
}
