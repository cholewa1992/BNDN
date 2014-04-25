using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ArtShare.Models
{
    public class MusicDetailsModel : IDetailsModel
    {
        public int ProductId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string FileExtension { get; set; }
        public string FileUrl { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Genres { get; set; }
        public string Artist { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string TrackLength { get; set; }
        public float? Price { get; set; }
        public int AccessRight { get; set; }
        public double AvgRating { get; set; }
        public int RatingsGiven { get; set; }
    }
}