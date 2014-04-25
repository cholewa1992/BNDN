using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ArtShare.Models
{
    public class UploadModel
    {
        public HttpPostedFileBase File { get; set; }


    }
}