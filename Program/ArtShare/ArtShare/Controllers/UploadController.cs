using ArtShare.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtShare.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(UploadModel um)
        {
            if (ModelState.IsValid)
            {
                TempData["success"] = um.File.FileName;
            }

            

            return RedirectToAction("Index", "Home");
        }

    }
}
