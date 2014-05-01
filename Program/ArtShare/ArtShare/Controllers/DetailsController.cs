using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Mvc;
using ArtShare.Logic;
using ShareItServices.MediaItemService;
using ArtShare.Models;

namespace ArtShare.Controllers
{
    /// <author>
    /// Mathias Pedersen (mkin@itu.dk)
    /// </author>
    public class DetailsController : Controller
    {

        private readonly IDetailsLogic _logic;

        public DetailsController()
        {
            _logic = new DetailsLogic();
        }

        public DetailsController(IDetailsLogic detailsLogic)
        {
            _logic = detailsLogic;
        }

        /// <summary>
        /// Show details view of media item with given Id
        /// </summary>
        /// <param name="id">The media item Id</param>
        /// <returns>The action result (view) to show</returns>
        /// /// GET Details/{id}
        public ActionResult Index(int id)
        {

            UserDTO user = null;

            if (Request.Cookies["user"] != null)
            {
                user = new UserDTO
                {
                    Id = int.Parse(Request.Cookies["user"].Values["id"]),
                    Username = Request.Cookies["user"].Values["username"],
                    Password = Request.Cookies["user"].Values["password"]
                };
            }

            try
            {
                MediaItemDTO dto = _logic.GetMediaItem(id, user);
                IDetailsModel model;
                switch (dto.Type)
                {

                    case MediaItemTypeDTO.Book:
                        model = _logic.ExtractBookInformation(dto);
                        break;

                    case MediaItemTypeDTO.Movie:
                        model = _logic.ExtractMovieInformation(dto);
                        break;

                    case MediaItemTypeDTO.Music:
                        model = _logic.ExtractMusicInformation(dto);
                        break;
                    default: throw new Exception("Dto type not known");

                }

                if(user != null) model.AccessRight = _logic.CheckAccessRights(new ShareItServices.AccessRightService.UserDTO { Id = user.Id, Username = user.Username, Password = user.Password }, id);

                TempData["model"] = model;
                return RedirectToAction(dto.Type.ToString(), "Details", new { id });
            }
            catch (FaultException e)
            {
                TempData["error"] = e;
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Showing book with given id
        /// </summary>
        /// <param name="id">The books id</param>
        /// <returns>The action result (view) to show</returns>
        /// GET Details/Book/{id}
        public ActionResult Book(int id)
        {
            if (TempData["model"] != null)
            {
                return  View(TempData["model"] as BookDetailsModel);
            }

            return Index(id);
        }

        
        
        /// <summary>
        /// Showing movie with given id
        /// </summary>
        /// <param name="id">The movies id</param>
        /// <returns>The action result (view) to show</returns>
        /// GET Details/Movie/{id}
        public ActionResult Movie(int id)
        {
            if (TempData["model"] != null)
            {
                return View(TempData["model"] as MovieDetailsModel);
            }
            return Index(id);
        }

        /// <summary>
        /// Showing track with given id
        /// </summary>
        /// <param name="id">The tracks id</param>
        /// <returns>The action result (view) to show</returns>
        /// GET Details/Music/{id}
        public ActionResult Music(int id)
        {
            if (TempData["model"] != null)
            {
                return View(TempData["model"] as MusicDetailsModel);
            }
            return Index(id);
        }
        
        /// <summary>
        /// Purcahses an media item
        /// </summary>
        /// <param name="mediaId">The id of the media to buy</param>
        /// <returns>The action result (view) to show</returns>
        public ActionResult PurchaseItem(int mediaId)
        {

            var userDto = new ShareItServices.AccessRightService.UserDTO();

            if (Request.Cookies["user"] != null)
            {
                userDto.Username = Request.Cookies["user"].Values["username"];
                userDto.Password = Request.Cookies["user"].Values["password"];
            }
            else
            {
                RedirectToAction("Index", "Login");
            }


            try
            {
                _logic.PurchaseItem(mediaId, userDto);
                TempData["success"] = "Item purchased!";
                return Index(mediaId);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return Index(mediaId);
            }
        }

        [HttpPost]
        public ActionResult RateMediaItem(int mediaId, int rating)
        {
            var userDto = new UserDTO();

            if (Request.Cookies["user"] != null)
            {
                userDto.Username = Request.Cookies["user"].Values["username"];
                userDto.Password = Request.Cookies["user"].Values["password"];
            }
            else
            {
                TempData["error"] = "In order to rate an item you must be logged in.";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                _logic.RateMediaItem(userDto, mediaId, rating);
                TempData["success"] = "Item has been rated!";
                return Index(mediaId);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return Index(mediaId);
            }
        }

        public ActionResult EditBook(int id)
        {
            if (TempData["model"] != null)
            {
                return View( TempData["model"] as BookDetailsModel);
            }
            return Edit(id);
        }

        public ActionResult EditMovie(int id)
        {
            if (TempData["model"] != null)
            {
                return View(TempData["model"] as MovieDetailsModel);
            }
            return Edit(id);
        }

        public ActionResult EditMusic(int id)
        {
            if (TempData["model"] != null)
            {
                return View(TempData["model"] as MusicDetailsModel);
            }
            return Edit(id);
        }

        public ActionResult Edit(int id)
        {

            UserDTO user = null;

            if (Request.Cookies["user"] != null)
            {
                user = new UserDTO
                {
                    Id = int.Parse(Request.Cookies["user"].Values["id"]),
                    Username = Request.Cookies["user"].Values["username"],
                    Password = Request.Cookies["user"].Values["password"]
                };
            }

            try
            {
                var dto = _logic.GetMediaItem(id, user);
                IDetailsModel model;
                switch (dto.Type)
                {

                    case MediaItemTypeDTO.Book:
                        model = _logic.ExtractBookInformation(dto);
                        break;

                    case MediaItemTypeDTO.Movie:
                        model = _logic.ExtractMovieInformation(dto);
                        break;

                    case MediaItemTypeDTO.Music:
                        model = _logic.ExtractMusicInformation(dto);
                        break;
                    default: throw new Exception("Dto type not known");

                }

                if (user != null) model.AccessRight = _logic.CheckAccessRights(new ShareItServices.AccessRightService.UserDTO { Id = user.Id, Username = user.Username, Password = user.Password }, id);

                TempData["model"] = model;
                return RedirectToAction("Edit" + dto.Type.ToString(), "Details", new { id });
            }
            catch (FaultException e)
            {
                TempData["error"] = e;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditBook(BookDetailsModel bookDetails)
        {

            var userDto = new UserDTO();

            if (Request.Cookies["user"] != null)
            {
                userDto.Username = Request.Cookies["user"].Values["username"];
                userDto.Password = Request.Cookies["user"].Values["password"];
            }
            else
            {
                TempData["error"] = "Login to edit book details";
                return RedirectToAction("Index", "Login");
            }


            try
            {
                _logic.EditBook(bookDetails, userDto);
                TempData["success"] = "The book's information has been updated";
                return Index(bookDetails.ProductId);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return Index(bookDetails.ProductId);
            }
        }

        [HttpPost]
        public ActionResult EditMovie(MovieDetailsModel movieDetails)
        {

            var userDto = new UserDTO();

            if (Request.Cookies["user"] != null)
            {
                userDto.Username = Request.Cookies["user"].Values["username"];
                userDto.Password = Request.Cookies["user"].Values["password"];
            }
            else
            {
                TempData["error"] = "Login to edit movie details";
                return RedirectToAction("Index", "Login");
            }


            try
            {
                _logic.EditMovie(movieDetails, userDto);
                TempData["success"] = "The movie's information has been updated";
                return Index(movieDetails.ProductId);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return Index(movieDetails.ProductId);
            }
        }

        [HttpPost]
        public ActionResult EditMusic(MusicDetailsModel musicDetails)
        {

            var userDto = new UserDTO();

            if (Request.Cookies["user"] != null)
            {
                userDto.Username = Request.Cookies["user"].Values["username"];
                userDto.Password = Request.Cookies["user"].Values["password"];
            }
            else
            {
                TempData["error"] = "Login to edit music details";
                return RedirectToAction("Index", "Login");
            }


            try
            {
                _logic.EditMusic(musicDetails, userDto);
                TempData["success"] = "The music's information has been updated";
                return Index(musicDetails.ProductId);
            }
            catch (ArgumentException e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return Index(musicDetails.ProductId);
            }
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            TempData["success"] = "Delete was called.";
            return RedirectToAction("Index", "Home");
        }
    }


}
