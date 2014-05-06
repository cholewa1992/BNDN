using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ArtShare.Models
{
    public class BookDetailsModel : IDetailsModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string FileExtension { get; set; }
        public string FileUrl { get; set; }
        public List<string> Tags { get; set; }
        public String TagsString { get; set; }
        public List<string> Genres { get; set; }
        public String GenresString { get; set; }
        [Display(Name=@"Number of pages")]
        public int? NumberOfPages { get; set; }
        public string Author { get; set; }
        [Display(Name=@"Release Date")]
        public DateTime? ReleaseDate { get; set; }
        public string Language { get; set; }
        public float? Price { get; set; }
        public int AccessRight { get; set; }
        public double AvgRating { get; set; }
        public int RatingsGiven { get; set; }
        [Display(Name = @"Uploader")]
        public string UploaderName { get; set; }
        public int UploaderId { get; set; }
    }
}