using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ArtShare.Models
{
    public class UploadModel
    {
        public HttpPostedFileBase File { get; set; }

        public HttpPostedFileBase Thumbnail { get; set; }

        public IDetailsModel Details { get; set; }


    }
}