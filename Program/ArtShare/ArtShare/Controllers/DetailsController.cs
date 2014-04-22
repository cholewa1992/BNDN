using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using ArtShare.Logic;
using Microsoft.Ajax.Utilities;
using ShareItServices.MediaItemService;
using ArtShare.Models;

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


        public ActionResult Stub()
        {

            /*var model = new Models.MovieDetailsModel()
            {
                AvgRating = 4,
                CastMembers = new List<string>{ "Jude Law", "Forest Whitaker", "Alice Braga", "Liev Schrieber" },
                Description = "In the near future, when artificial human organs can be bought on credit, one man makes a living by repossessing organs their users can't pay for. What happens when the best repo man in the business can't make payments on his own artificial heart?",
                Director = "Miguel Sapochnik",
                FileExtension = ".mp4",
                FileUrl = "http://image.tmdb.org/t/p/w396/juU0imfyYlwfkKFdwE7ZMvLf16u.jpg",
                Genres = new List<string> { "Action", "Science Fiction", "Thriller", "Crime" },
                Language = "English (en)",
                Price = 299,
                ReleaseDate = new DateTime(2010, 03, 19),
                Runtime = "111",
                Tags = new List<string>{ "Evil Corporation", "Repo Man", "Aftercreditsstringer" },
                Thumbnail = "http://image.tmdb.org/t/p/w396/juU0imfyYlwfkKFdwE7ZMvLf16u.jpg",
                Title = "Repo Men"
            };*/

            /*var model = new Models.MusicDetailsModel()
            {
                Artist = "Miley Cyrus",
                Description = "Wrecking Ball is a song performed by American recording artist Miley Cyrus for her fourth studio album Bangerz (2013). It was released on August 25, 2013, by RCA Records as the second single from the record. It was written by MoZella, Stephan Moccio, Sacha Skarbek, Lukasz Gottwald, and Henry Russell Walter. It was produced by Dr. Luke and Cirkut. Wrecking Ball is a pop ballad which lyrically discusses the deterioration of a relationship; it has been widely speculated to have been inspired by Cyrus' former fiancé Liam Hemsworth.",
                FileExtension = ".mp3",
                FileUrl = "http://upload.wikimedia.org/wikipedia/en/thumb/0/06/Miley_Cyrus_-_Wrecking_Ball.jpg/220px-Miley_Cyrus_-_Wrecking_Ball.jpg",
                Genres = new List<string> { "Pop" },
                Price = 200,
                ReleaseDate = new DateTime(2013, 08, 25),
                Tags = new List<string> { "A tag", "Another tag" },
                Thumbnail = "http://upload.wikimedia.org/wikipedia/en/thumb/0/06/Miley_Cyrus_-_Wrecking_Ball.jpg/220px-Miley_Cyrus_-_Wrecking_Ball.jpg",
                Title = "Wrecking Ball",
                TrackLength = "3:41"
            };*/

            var model = new Models.BookDetailsModel()
            {
                Author = "Arthur Conan Doyle",
                Description = "A Study in Scarlet is a detective mystery novel written by Sir Arthur Conan Doyle, introducing his new characters, consulting detective Sherlock Holmes and his friend and chronicler, Dr. John Watson, who later became two of the most famous characters in literature. Conan Doyle wrote the story in 1886, and it was published the following year. The book's title derives from a speech given by Holmes to Doctor Watson on the nature of his work, in which he describes the story's murder investigation as his 'study in scarlet': 'There's the scarlet thread of murder running through the colourless skein of life, and our duty is to unravel it, and isolate it, and expose every inch of it.' (A 'study' is a preliminary drawing, sketch or painting done in preparation for a finished piece.)",
                FileExtension = ".pdf",
                FileUrl = "http://en.wikipedia.org/wiki/A_Study_in_Scarlet",
                Genres = new List<string> { "Detective", "Crime", "Mystery", "Novel" },
                Language = "English",
                NumberOfPages = 96,
                Price = 50,
                ReleaseDate = new DateTime(1887),
                Tags = new List<string> { "a tag", "Arthur" },
                Thumbnail = "http://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/ArthurConanDoyle_AStudyInScarlet_annual.jpg/220px-ArthurConanDoyle_AStudyInScarlet_annual.jpg",
                Title = "A Study in Scarlet"
            };

            return View(model);

        }

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
                object model;
                switch (dto.Type)
                {

                    case MediaItemTypeDTO.Book:
                        model = _logic.ExstractBookInformation(dto);
                        break;

                    case MediaItemTypeDTO.Movie:
                        model = _logic.ExstractMovieInformation(dto);
                        break;

                    case MediaItemTypeDTO.Music:
                        model = _logic.ExstractMusicInformation(dto);
                        break;
                    default: throw new Exception("Dto type not known");

                }
                TempData["model"] = model;
                return RedirectToAction(dto.Type.ToString(), "Details", new { id });
            }
            catch (FaultException e)
            {
                TempData["error"] = e;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET Details/Book/{id}
        public ActionResult Book(int id)
        {
            if (TempData["model"] != null)
            {
                return View(TempData["model"] as BookDetailsModel);
            }
            return Index(id);
        }

        // GET Details/Movie/{id}
        public ActionResult Movie(int id)
        {
            if (TempData["model"] != null)
            {
                return View(TempData["model"] as MovieDetailsModel);
            }
            return Index(id);
        }

        // GET Details/Music/{id}
        public ActionResult Music(int id)
        {
            if (TempData["model"] != null)
            {
                return View(TempData["model"] as MusicDetailsModel);
            }
            return Index(id);
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
