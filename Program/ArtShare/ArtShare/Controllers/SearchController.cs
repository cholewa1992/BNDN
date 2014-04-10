using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.AccessRightService;
using ShareItServices.MediaItemService;

namespace ArtShare.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index()
        {
            return Index();
            //return View();
        }

        [HttpPost]
        public ActionResult SearchMediaItems(int from, int to, string searchKey)
        {
            using (var client = new MediaItemServiceClient())
            {
                try
                {
                    var searchResult = client.SearchMediaItems(from, to, searchKey, Resources.ClientToken);
                    var model = PrepareSearchModel(new SearchModel(), searchResult);
                    return Index();
                    //return View(model);
                }
                //TODO Handle exceptions
                catch (FaultException<ArgumentFault> e)
                {
                    return null;
                }
                catch (FaultException<UnauthorizedClient> e)
                {
                    return null;
                }
                /*catch (FaultException<MediaItemNotFound> e)
                {
                    return null;
                }*/
                catch (FaultException e)
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public ActionResult SearchMediaItemsByType(int from, int to, MediaItemTypeDTO type, string searchKey)
        {
            using (var client = new MediaItemServiceClient())
            {
                try
                {
                    var searchResult = client.SearchMediaItemsByType(from, to, type, searchKey, 
                        Resources.ClientToken);
                    var model = PrepareSearchModel(new SearchModel(), searchResult);
                    return Index();
                    //return View(model);
                }
                //TODO Handle exceptions
                catch (FaultException<ArgumentFault> e)
                {
                    return null;
                }
                catch (FaultException<UnauthorizedClient> e)
                {
                    return null;
                }
                /*catch (FaultException<MediaItemNotFound> e)
                {
                    return null;
                }*/
                catch (FaultException e)
                {
                    return null;
                }
            }
        }

        private SearchModel PrepareSearchModel(SearchModel model, Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> searchResult)
        {
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
