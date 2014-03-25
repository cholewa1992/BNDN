using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Linq;
using BusinessLogicLayer.DTO;
using DataAccessLayer;

namespace BusinessLogicLayer.Stub
{
    public class MediaItemLogicStub : IMediaItemLogic
    {

        /// <summary>
        ///     Returns a stub media item with a collection of media item information
        /// </summary>
        /// <param name="mediaItemId">The id of the media item</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItemDTO GetMediaItemInformation(int mediaItemId, string clientToken)
        {
            var list = new List<MediaItemInformationDTO> {
                new MediaItemInformationDTO
                {
                    Id = 1,
                    Type = InformationTypeDTO.Title,
                    Data = "Harry Potter And The Chamber Of Secrets"
                },
                new MediaItemInformationDTO
                {
                    Id = 1,
                    Type = InformationTypeDTO.Price,
                    Data = "6.64"
                },
                new MediaItemInformationDTO
                {
                    Id = 1,
                    Type = InformationTypeDTO.NumberOfPages,
                    Data = "341"
                },
                new MediaItemInformationDTO
                {
                    Id = 1,
                    Type = InformationTypeDTO.Genre,
                    Data = "Fantasy"
                },
                new MediaItemInformationDTO
                {
                    Id = 1,
                    Type = InformationTypeDTO.Author,
                    Data = "J.K. Rowling"
                }
            };

            var item = new MediaItemDTO {Id = 1, Type = MediaItemTypeDTO.Book, FileExtension = ".pdf", Information = list};

            return item;
        }

        public Dictionary<MediaItemTypeDTO, List<MediaItemDTO>> FindMediaItemRange(int from, int to, MediaItemTypeDTO? mediaType,
            string searchKey, string clientToken)
        {
            throw new NotImplementedException();
        }
    }
}
