using ArtShare.Logic;
using ArtShare.Models;
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
            var model = _searchLogic.GetMediaItems(1, 100);

            model.Books = GetFeaturedItems<BookDetailsModel>(model.Books);
            model.Movies = GetFeaturedItems<MovieDetailsModel>(model.Movies);
            model.Music = GetFeaturedItems<MusicDetailsModel>(model.Music);
            
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

        private List<T> GetFeaturedItems<T> (List<T> list) where T : IDetailsModel
        {
            List<T> returnList = new List<T>();
            
            if(list.Count > 6){
                

                for(var i = 0; i < 6; i++){

                    bool found = false;
                    while(!found){
                        var r = new Random().Next(1, list.Count);
                        if(list[r-1] != null && !returnList.Contains(list[r-1])){
                            returnList.Add(list[r-1]);
                            found = true;
                        }
                    }
                }
                list = returnList;
            }

            return list;
        }

    }
}
