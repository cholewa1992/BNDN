using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ArtShare.Models
{
    public class UploadMovieModel : IUploadModel
    {
        public HttpPostedFileBase File { get; set; }

        public HttpPostedFileBase Thumbnail { get; set; }

        public MovieDetailsModel Details { get; set; }


    }
}