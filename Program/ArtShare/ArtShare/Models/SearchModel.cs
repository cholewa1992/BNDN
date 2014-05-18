using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShareItServices.MediaItemService;

namespace ArtShare.Models
{
    /// <author>Thomas Dragsbæk (thst@itu.dk)</author>
    public class SearchModel
    {
        //Books
        [Display(Name = "Books")]
        public List<BookDetailsModel> Books { get; set; }

        [Display(Name = "Number of matching books")]
        public int NumberOfMatchingBooks { get; set; }

        //Music
        [Display(Name = "Music")]
        public List<MusicDetailsModel> Music { get; set; }

        [Display(Name = "Number of matching music files")]
        public int NumberOfMatchingMusic { get; set; }

        //Movies
        [Display(Name = "Movies")]
        public List<MovieDetailsModel> Movies { get; set; }

        [Display(Name = "Number of matching movies")]
        public int NumberOfMatchingMovies { get; set; }
    }
}