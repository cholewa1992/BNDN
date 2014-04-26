using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ArtShare.Models
{
    public class UploadMusicModel : IUploadModel
    {
        public HttpPostedFileBase File { get; set; }

        public HttpPostedFileBase Thumbnail { get; set; }

        public MusicDetailsModel Details { get; set; }


    }
}