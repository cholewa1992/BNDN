using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtShare.Models;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public class SearchLogicStub : ISearchLogic
    {
        private List<MediaItemDTO> testMediaItems;

        public SearchLogicStub()
        {
            testMediaItems = Setup();
        }

        private List<MediaItemDTO> Setup()
        {
            var result = new List<MediaItemDTO>();
            int count = 1;
            for (int iBook = 0; iBook <= 4; iBook++)
            {
                result.Add(new MediaItemDTO
                {
                    Type = MediaItemTypeDTO.Book,
                    Information = new MediaItemInformationDTO[]{ new MediaItemInformationDTO
                    {
                        Id = count++, 
                        Data = "Book " + iBook 
                    }
                }});
            }

            for (int iMovie = 0; iMovie <= 3; iMovie++)
            {
                result.Add(new MediaItemDTO
                {
                    Type = MediaItemTypeDTO.Movie,
                    Information = new MediaItemInformationDTO[]{ new MediaItemInformationDTO
                    {
                        Id = count++, 
                        Data = "Movie " + iMovie 
                    }
                }});
            }

            for (int iMusic = 0; iMusic <= 4; iMusic++)
            {
                result.Add(new MediaItemDTO
                {
                    Type = MediaItemTypeDTO.Music,
                    Information = new MediaItemInformationDTO[]{ new MediaItemInformationDTO
                    {
                        Id = count++, 
                        Data = "Music " + iMusic 
                    }
                }});
            }
            return result;
        } 

        public SearchModel SearchMediaItems(int from, int to, string searchKey)
        {
            return new SearchModel
            {
                Books = new List<MediaItemDTO> { 
                    new MediaItemDTO {Type = MediaItemTypeDTO.Book }, 
                    new MediaItemDTO {Type = MediaItemTypeDTO.Book } },
                Movies = new List<MediaItemDTO> { new MediaItemDTO { Type = MediaItemTypeDTO.Movie } },
                Music = new List<MediaItemDTO> { new MediaItemDTO { Type = MediaItemTypeDTO.Music } },
                NumberOfMatchingBooks = 2,
                NumberOfMatchingMovies = 1,
                NumberOfMatchingMusic = 1
            };
        }

        public SearchModel SearchMediaItemsByType(int from, int to, MediaItemTypeDTO type, string searchKey)
        {
            SearchModel model = null;
            var searchResult = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();
            switch (type)
            {
                case MediaItemTypeDTO.Book:
                    var bookSearchResult = new MediaItemSearchResultDTO
                    {
                        MediaItemList = testMediaItems.Where(x => x.Type == MediaItemTypeDTO.Book &&
                            x.Information.Any(info => info.Data.Contains(searchKey))).Select(x => x).ToArray(),
                        NumberOfSearchResults = testMediaItems.Count(x => x.Type == MediaItemTypeDTO.Book)
                    };
                    searchResult.Add(MediaItemTypeDTO.Book, bookSearchResult);
                    model = PrepareSearchModel(searchResult);
                    break;
                case MediaItemTypeDTO.Movie:
                    var movieSearchResult = new MediaItemSearchResultDTO
                    {
                        MediaItemList = testMediaItems.Where(x => x.Type == MediaItemTypeDTO.Movie &&
                            x.Information.Any(info => info.Data.Contains(searchKey))).Select(x => x).ToArray(),
                        NumberOfSearchResults = testMediaItems.Count(x => x.Type == MediaItemTypeDTO.Movie)
                    };
                    searchResult.Add(MediaItemTypeDTO.Book, movieSearchResult);
                    model = PrepareSearchModel(searchResult);
                    break;
                case MediaItemTypeDTO.Music:
                    var musicSearchResult = new MediaItemSearchResultDTO
                    {
                        MediaItemList = testMediaItems.Where(x => x.Type == MediaItemTypeDTO.Music &&
                            x.Information.Any(info => info.Data.Contains(searchKey))).Select(x => x).ToArray(),
                        NumberOfSearchResults = testMediaItems.Count(x => x.Type == MediaItemTypeDTO.Music)
                    };
                    searchResult.Add(MediaItemTypeDTO.Book, musicSearchResult);
                    model = PrepareSearchModel(searchResult);
                    break;
            }
            return model;
        }

        private SearchModel PrepareSearchModel(Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> searchResult)
        {
            var model = new SearchModel();
            foreach (var pair in searchResult)
            {
                var mediaItemList = pair.Value.MediaItemList.ToList();
                var numberOfSearchResults = pair.Value.NumberOfSearchResults;
                switch ((int)pair.Key)
                {
                    case (int)MediaItemTypeDTO.Book:
                        model.Books = mediaItemList;
                        model.NumberOfMatchingBooks = numberOfSearchResults;
                        break;
                    case (int)MediaItemTypeDTO.Music:
                        model.Music = mediaItemList;
                        model.NumberOfMatchingMusic = numberOfSearchResults;
                        break;
                    case (int)MediaItemTypeDTO.Movie:
                        model.Movies = mediaItemList;
                        model.NumberOfMatchingMovies = numberOfSearchResults;
                        break;
                }
            }
            return model;
        }
    }
}