using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ArtShare.Models
{
    public class UploadBookModel : IUploadModel
    {
        public HttpPostedFileBase File { get; set; }

        public HttpPostedFileBase Thumbnail { get; set; }

        public BookDetailsModel Details { get; set; }


    }
}