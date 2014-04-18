using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;
using Microsoft.Ajax.Utilities;
using ShareItServices.MediaItemService;

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

        public ActionResult Index(int id)
        {

            UserDTO user = null;

            if (Request.Cookies["user"] != null)
            {
                user = new UserDTO()
                {
                    Username = Request.Cookies["user"].Values["username"]
                };
            }

            MediaItemDTO dto;

            try
            {
                dto = _logic.GetMediaItem(id, user);

                switch (dto.Type)
                {

                    case MediaItemTypeDTO.Book:
                        var bookModel = _logic.ExstractBookInformation(dto);
                        return View(bookModel);

                    case MediaItemTypeDTO.Movie:
                        var movieModel = _logic.ExstractBookInformation(dto);
                        return View(movieModel);

                    case MediaItemTypeDTO.Music:
                        var musicModel = _logic.ExstractBookInformation(dto);
                        return View(musicModel);

                }
            }
            catch (FaultException e)
            {
                TempData["error"] = e;
                return View();
            }

            return View();
        }



        ///// <summary>
        ///// Build view with Book details model
        ///// GET: /Details/BookDetails/5
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult BookDetails(int id)
        //{
        //    //TODO collapse into one details get

        //    int? userId = null;

        //    if (Request.Cookies["user"] != null)
        //    {
        //        userId = int.Parse(Request.Cookies["user"].Values["id"]);
        //    }

        //    try
        //    {
        //        var model = _logic.GetBookDetailsModel(id, userId);
        //        return View(model);
        //    }
        //    catch (Exception)
        //    {
        //        //TODO error view
        //    }

        //    return View();
        //}

        ///// <summary>
        ///// Build view with Movie details model
        ///// GET: /Details/MovieDetails/5
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult MovieDetails(int id)
        //{
        //    int? userId = null;

        //    if (Request.Cookies["user"] != null)
        //    {
        //        userId = int.Parse(Request.Cookies["user"].Values["id"]);
        //    }

        //    try
        //    {
        //        var model = _logic.GetMovieDetailsModel(id, userId);
        //        return View(model);
        //    }
        //    catch (Exception)
        //    {
        //        //TODO error view
        //    }

        //    return View();
        //}

        ///// <summary>
        ///// Build view with music details model
        ///// GET: /Details/MusicDetails/5
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult MusicDetails(int id)
        //{
        //    int? userId = null;

        //    if (Request.Cookies["user"] != null)
        //    {
        //        userId = int.Parse(Request.Cookies["user"].Values["id"]);
        //    }

        //    try
        //    {
        //        var model = _logic.GetMusicDetailsModel(id, userId);
        //        return View(model);
        //    }
        //    catch (Exception)
        //    {
        //        //TODO error view
        //    }

        //    return View();
        //}


        public ActionResult PurchaseItem(int mediaId)
        {

            int userId = -1;

            if (Request.Cookies["user"] != null)
            {
                userId = int.Parse(Request.Cookies["user"].Values["id"]);
            }
            else
            {
                RedirectToAction("Index", "Login");
            }


            try
            {
                _logic.PurchaseItem(mediaId, userId);
                //TODO Redirect to download
                return View();
            }
            catch (Exception)
            {

                return View();
            }


        }

    }
}
