using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ArtShare.Models
{
    public class MovieDetailsModel : AbstractDetailsModel
    {

        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string FileExtension { get; set; }
        public string FileUrl { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Genres { get; set; }
        public string Director { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Runtime { get; set; }
        public List<string> CastMembers { get; set; }
        public float AvgRating { get; set; }
        public string Language { get; set; }
        public float? Price { get; set; }
        public int AccessRight { get; set; }

    }
}