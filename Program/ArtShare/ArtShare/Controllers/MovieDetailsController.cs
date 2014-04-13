using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;
using ArtShare.Models;

namespace ArtShare.Controllers
{
    public class MovieDetailsController : Controller
    {

        private IMovieLogic _logic;

        public MovieDetailsController()
        {
            _logic = new MovieLogic();
        }

        public MovieDetailsController(IMovieLogic movieLogic)
        {
            _logic = movieLogic;
        }



        //
        // GET: /MovieDetails/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /MovieDetails/Details/5

        public ActionResult Details(int id)
        {

            int? userId = null;
            
            if (Request.Cookies["user"] != null)
            {
                userId = int.Parse(Request.Cookies["user"].Value);
            }



            try
            {
                var movieDetails = _logic.GetMovieDetailsModel(id, userId);
            }
            catch (Exception e)
            {
                
            }

            return View();
        }

        //
        // GET: /MovieDetails/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MovieDetails/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /MovieDetails/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /MovieDetails/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /MovieDetails/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /MovieDetails/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
