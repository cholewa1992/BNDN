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
        /// <param name="userId">The id of the user requesting the media item. Null is allowed and can be used if the user is not logged in</param>
        /// <param name="clientToken">Token used to verify the client</param>
        /// <returns>A MediaItem with all its information</returns>
        public MediaItemDTO GetMediaItemInformation(int mediaItemId, int? userId, string clientToken)
        {
            var list = new List<MediaItemInformationDTO> {
                new MediaItemInformationDTO
                {
                    Id = 1,
                    Type = InformationTypeDTO.Title,
                    Data = "Harry Potter And The Chamber Of Secrets",
                },
                new MediaItemInformationDTO
                {
                    Id = 2,
                    Type = InformationTypeDTO.Price,
                    Data = "6.64"
                },
                new MediaItemInformationDTO
                {
                    Id = 3,
                    Type = InformationTypeDTO.NumberOfPages,
                    Data = "341"
                },
                new MediaItemInformationDTO
                {
                    Id = 4,
                    Type = InformationTypeDTO.Genre,
                    Data = "Fantasy"
                },
                new MediaItemInformationDTO
                {
                    Id = 5,
                    Type = InformationTypeDTO.Author,
                    Data = "J.K. Rowling"
                }
            };

            var item = new MediaItemDTO {Id = 1, Type = MediaItemTypeDTO.Book, FileExtension = ".pdf", Information = list};

            return item;
        }

        public Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> FindMediaItemRange(int @from, int to, MediaItemTypeDTO? mediaType, string searchKey, string clientToken)
        {
            var result = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();

            var list1 = new List<MediaItemInformationDTO> {
                new MediaItemInformationDTO
                {
                    Id = 1,
                    Type = InformationTypeDTO.Title,
                    Data = "Harry Potter And The Chamber Of Secrets"
                },
                new MediaItemInformationDTO
                {
                    Id = 2,
                    Type = InformationTypeDTO.Price,
                    Data = "6.64"
                },
                new MediaItemInformationDTO
                {
                    Id = 3,
                    Type = InformationTypeDTO.NumberOfPages,
                    Data = "341"
                },
                new MediaItemInformationDTO
                {
                    Id = 4,
                    Type = InformationTypeDTO.Genre,
                    Data = "Fantasy"
                },
                new MediaItemInformationDTO
                {
                    Id = 5,
                    Type = InformationTypeDTO.Author,
                    Data = "J.K. Rowling"
                }
            };
            var book1 = new MediaItemDTO {Id = 1, Type = MediaItemTypeDTO.Book, FileExtension = ".pdf", Information = list1};

            var list2 = new List<MediaItemInformationDTO> {
                new MediaItemInformationDTO
                {
                    Id = 6,
                    Type = InformationTypeDTO.Title,
                    Data = "The Lost Symbol"
                },
                new MediaItemInformationDTO
                {
                    Id = 7,
                    Type = InformationTypeDTO.Price,
                    Data = "9.43"
                },
                new MediaItemInformationDTO
                {
                    Id = 8,
                    Type = InformationTypeDTO.Author,
                    Data = "Dan Brown"
                }
            };
            var book2 = new MediaItemDTO {Id = 2, Type = MediaItemTypeDTO.Book, FileExtension = ".pdf", Information = list2};

            var bookList = new List<MediaItemDTO> {book1, book2};
            var bookSearchResult = new MediaItemSearchResultDTO 
            {
                MediaItemList = bookList, 
                NumberOfSearchResults = bookList.Count
            };

            var list3 = new List<MediaItemInformationDTO>
            {
                new MediaItemInformationDTO
                {
                    Id = 9,
                    Type = InformationTypeDTO.Artist,
                    Data = "Pharrell Williams"
                }, 
                new MediaItemInformationDTO
                {
                    Id = 10,
                    Type = InformationTypeDTO.Title,
                    Data = "Happy"
                }, 
                new MediaItemInformationDTO
                {
                    Id = 11,
                    Type = InformationTypeDTO.TrackLength,
                    Data = "4:08"
                }
            };
            var music1 = new MediaItemDTO
            {
                Id = 3, 
                Type = MediaItemTypeDTO.Music, 
                FileExtension = ".mp3", 
                Information = list3
            };
            var musicList = new List<MediaItemDTO> {music1};
            var musicSearchResult = new MediaItemSearchResultDTO()
            {
                MediaItemList = musicList,
                NumberOfSearchResults = musicList.Count
            };

            var list4 = new List<MediaItemInformationDTO>
            {
                new MediaItemInformationDTO
                {
                    Id = 12,
                    Type = InformationTypeDTO.Director,
                    Data = "Quentin Tarantino"
                }, 
                new MediaItemInformationDTO
                {
                    Id = 13,
                    Type = InformationTypeDTO.Title,
                    Data = "Django Unchained"
                }, 
                new MediaItemInformationDTO
                {
                    Id = 14,
                    Type = InformationTypeDTO.Runtime,
                    Data = "165 minutes"
                },
                new MediaItemInformationDTO
                {
                    Id = 15,
                    Type = InformationTypeDTO.ReleaseDate,
                    Data = "25.12.2012"
                }
            };
            var movie1 = new MediaItemDTO
            {
                Id = 4, 
                Type = MediaItemTypeDTO.Movie, 
                FileExtension = ".avi", 
                Information = list4
            };
            var movieList = new List<MediaItemDTO> {movie1};
            var movieSearchResult = new MediaItemSearchResultDTO()
            {
                MediaItemList = movieList,
                NumberOfSearchResults = movieList.Count
            };

            result.Add(MediaItemTypeDTO.Book, bookSearchResult);
            result.Add(MediaItemTypeDTO.Music, musicSearchResult);
            result.Add(MediaItemTypeDTO.Movie, movieSearchResult);

            return result;
        }
    }
}
