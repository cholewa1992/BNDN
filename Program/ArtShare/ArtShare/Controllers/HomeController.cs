using ArtShare.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtShare.Controllers
{
    public class HomeController : Controller
    {

        private ISearchLogic _searchLogic;

        public HomeController()
        {
            _searchLogic = new SearchLogic();
        }

        public HomeController(ISearchLogic logic)
        {
            _searchLogic = logic;
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var model = _searchLogic.GetMediaItems(1, 5);
            return View(model);
        }

        public ActionResult Movies()
        {
            return View();
        }

        public ActionResult Music()
        {
            return View();
        }

        public ActionResult Books()
        {
            return View();
        }

    }
}
