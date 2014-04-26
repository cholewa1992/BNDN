using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ArtShare.Models
{
    public interface IUploadModel
    {
        HttpPostedFileBase File { get; set; }

        HttpPostedFileBase Thumbnail { get; set; }

        IDetailsModel Details { get; set; }


    }
}