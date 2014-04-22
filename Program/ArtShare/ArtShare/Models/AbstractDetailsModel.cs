using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtShare.Models
{
    public interface AbstractDetailsModel
    {
        int ProductId { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Thumbnail { get; set; }
        string FileExtension { get; set; }
        string FileUrl { get; set; }
        List<string> Tags { get; set; }
        List<string> Genres { get; set; }
        float? Price { get; set; }
        int AccessRight { get; set; }
    }
}