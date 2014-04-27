using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ArtShare
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "rate",
                "Details/RateMediaItem",
                new { controller = "Details", action = "RateMediaItem"}
            );

            routes.MapRoute(
                "details",
                "Details/{id}",
                new { controller = "Details", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "download",
                "Download/{id}",
                new { controller = "Download", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "movies",
                "Movies/{id}",
                new { controller = "Home", action = "Movies", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "music",
                "Music/{id}",
                new { controller = "Home", action = "Music", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "books",
                "Books/{id}",
                new { controller = "Home", action = "Books", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}