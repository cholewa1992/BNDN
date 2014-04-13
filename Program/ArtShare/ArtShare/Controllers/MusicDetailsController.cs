using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;

namespace ArtShare.Controllers
{
    public class MusicDetailsController : Controller
    {

        private IMusicLogic _logic;


        public MusicDetailsController()
        {
            _logic = new MusicLogic();
        }

        public MusicDetailsController(IMusicLogic musicLogic)
        {
            _logic = musicLogic;
        }



        //
        // GET: /MusicDetails/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /MusicDetails/Details/5

        public ActionResult Details(int id)
        {
            int? userId = null;

            if (Request.Cookies["user"] != null)
            {
                userId = int.Parse(Request.Cookies["user"].Value);
            }

            try
            {
                var model = _logic.GetMusicDetailsModel(id, userId);
            }
            catch (Exception)
            {
                
                throw;
            }

            return View();
        }

        //
        // GET: /MusicDetails/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MusicDetails/Create

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
        // GET: /MusicDetails/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /MusicDetails/Edit/5

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
        // GET: /MusicDetails/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /MusicDetails/Delete/5

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
