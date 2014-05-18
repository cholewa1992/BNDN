using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.AccessRightService;
using ShareItServices.MediaItemService;

namespace ArtShare.Controllers
{
    /// <author>Thomas Dragsbæk (thst@itu.dk)</author>
    public class SearchController : Controller
    {
        private ISearchLogic _searchLogic;

        public SearchController()
        {
            _searchLogic = new SearchLogic();
        }

        public SearchController(ISearchLogic logic)
        {
            _searchLogic = logic;
        }
        
        //
        // GET: /Search/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Search/SearchMediaItems
        [HttpGet]
        public ActionResult SearchMediaItems(int from, int to, string searchKey)
        {
            try
            {
                var model = _searchLogic.SearchMediaItems(from, to, searchKey);
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Search/SearchMediaitemsByType
        [HttpGet]
        public ActionResult SearchMediaItemsByType(int from, int to, MediaItemTypeDTO type, string searchKey)
        {
            try
            {
                var model = _searchLogic.SearchMediaItems(from, to, searchKey);
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
