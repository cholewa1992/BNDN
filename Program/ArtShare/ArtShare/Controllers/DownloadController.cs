using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtShare.ActionResults;
using ArtShare.Logic;
using ShareItServices.TransferService;

namespace ArtShare.Controllers
{
    public class DownloadController : Controller
    {

        private ITransferLogic _logic;

        public DownloadController()
        {
            _logic = new TransferLogic();
        }

        public DownloadController(ITransferLogic logic)
        {
            _logic = logic;
        }


        //
        // GET: /Download/5

        public ActionResult Index(int id, string fileName)
        {

            //Checking that the user is logged in
            var userCookie = Request.Cookies["user"];
            if (userCookie == null)
            {
                TempData["error"] = "You have to be logged in to download files.";
                return RedirectToAction("Index", "Home");
            }

            var user = new UserDTO() { Username = userCookie["username"], Password = userCookie["password"] };
            //Default filename to the id
            if (String.IsNullOrWhiteSpace(fileName))
                fileName = id.ToString(CultureInfo.InvariantCulture);
            try
            {
                string fileExtension;
                long fileLength;
                var stream = _logic.DownloadFile(user, id, out fileExtension, out fileLength);
                fileName = fileName + fileExtension;

                var result = new StreamedFileResult(stream, MimeMapping.GetMimeMapping(fileName), fileName);
                result.FileSize = fileLength;
                return result;
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Details", new {id = id});
            }
            
        }

    }
}
