using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;

namespace ArtShare.Controllers
{
    public class DetailsController : Controller
    {

        private IDetailsLogic _logic;

        public DetailsController()
        {
            _logic = new DetailsLogic();
        }

        public DetailsController(IDetailsLogic detailsLogic)
        {
            _logic = detailsLogic;
        }

        //
        // GET: /Details/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Build view with Book details model
        /// GET: /Details/BookDetails/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BookDetails(int id)
        {
            //TODO collapse into one details get

            int? userId = null;

            if (Request.Cookies["user"] != null)
            {
                userId = int.Parse(Request.Cookies["user"].Value);
            }

            try
            {
                var model = _logic.GetMovieDetailsModel(id, userId);
                return View(model);
            }
            catch (Exception)
            {
                //TODO error view
            }

            return View();
        }

        /// <summary>
        /// Build view with Movie details model
        /// GET: /Details/MovieDetails/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MovieDetails(int id)
        {
            int? userId = null;

            if (Request.Cookies["user"] != null)
            {
                userId = int.Parse(Request.Cookies["user"].Value);
            }

            try
            {
                var model = _logic.GetMovieDetailsModel(id, userId);
                return View(model);
            }
            catch (Exception)
            {
                //TODO error view
            }

            return View();
        }

        /// <summary>
        /// Build view with music details model
        /// GET: /Details/MusicDetails/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MusicDetails(int id)
        {
            int? userId = null;

            if (Request.Cookies["user"] != null)
            {
                userId = int.Parse(Request.Cookies["user"].Value);
            }

            try
            {
                var model = _logic.GetMusicDetailsModel(id, userId);
                return View(model);
            }
            catch (Exception)
            {
                //TODO error view
            }

            return View();
        }

    }
}
