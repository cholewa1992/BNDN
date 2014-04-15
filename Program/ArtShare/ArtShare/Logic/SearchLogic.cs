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

        public SearchModel PrepareSearchModel(Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> searchResult)
        {
            if (searchResult == null) { throw new ArgumentNullException("searchResult"); }

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
                            model.Books = mediaItemList;
                            model.NumberOfMatchingBooks = numberOfSearchResults;
                            break;
                        case (int) MediaItemTypeDTO.Music:
                            model.Music = mediaItemList;
                            model.NumberOfMatchingMusic = numberOfSearchResults;
                            break;
                        case (int) MediaItemTypeDTO.Movie:
                            model.Movies = mediaItemList;
                            model.NumberOfMatchingMovies = numberOfSearchResults;
                            break;
                    }
                }
            }
            return model;
        }
    }
}