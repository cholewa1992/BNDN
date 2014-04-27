using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel;
using System.Web;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.AccessRightService;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public class SearchLogic : ISearchLogic
    {    
        public SearchModel SearchMediaItems(int from, int to, string searchKey)
        {
            using (var client = new MediaItemServiceClient())
            {
                var searchResult = client.SearchMediaItems(from, to, searchKey, Resources.ClientToken);
                return PrepareSearchModel(searchResult);
            }
        }

        public SearchModel SearchMediaItemsByType(int from, int to, MediaItemTypeDTO type, string searchKey)
        {
            using (var client = new MediaItemServiceClient())
            {
                var searchResult = client.SearchMediaItemsByType(from, to, type, searchKey, Resources.ClientToken);
                return PrepareSearchModel(searchResult);
            }
        }

        public SearchModel GetMediaItems(int from, int to)
        {
            using (var client = new MediaItemServiceClient())
            {
                var searchResult = client.GetMediaItems(from, to, Resources.ClientToken);
                return PrepareSearchModel(searchResult);
            }
        }

        public SearchModel GetMediaItemsByType(int from, int to, MediaItemTypeDTO type)
        {
            using (var client = new MediaItemServiceClient())
            {
                var searchResult = client.GetMediaItemsByType(from, to, type, Resources.ClientToken);
                return PrepareSearchModel(searchResult);
            }
        }

        public SearchModel PrepareSearchModel(Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> searchResult)
        {
            if (searchResult == null) { throw new ArgumentNullException("searchResult"); }

            var detailsLogic = new DetailsLogic();
            var model = new SearchModel();
            foreach (var pair in searchResult)
            {
                if (pair.Value != null)
                {
                    var mediaItemList = pair.Value.MediaItemList.ToList();
                    var numberOfSearchResults = pair.Value.NumberOfSearchResults;
                    switch ((int) pair.Key)
                    {
                        case (int) MediaItemTypeDTO.Book:
                            var books = mediaItemList.Select(item => detailsLogic.ExtractBookInformation(item)).ToList();
                            model.Books = books;
                            model.NumberOfMatchingBooks = numberOfSearchResults;
                            break;
                        case (int) MediaItemTypeDTO.Music:
                            var music = mediaItemList.Select(item => detailsLogic.ExtractMusicInformation(item)).ToList();
                            model.Music = music;
                            model.NumberOfMatchingMusic = numberOfSearchResults;
                            break;
                        case (int) MediaItemTypeDTO.Movie:
                            var movies = mediaItemList.Select(item => detailsLogic.ExtractMovieInformation(item)).ToList();
                            model.Movies = movies;
                            model.NumberOfMatchingMovies = numberOfSearchResults;
                            break;
                    }
                }
            }
            return model;
        }
    }
}