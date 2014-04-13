﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShareItServices.MediaItemService;

namespace ArtShare.Models
{
    public class SearchModel
    {
        //Books
        [Display(Name = "Books")]
        public List<MediaItemDTO> Books { get; set; }

        [Display(Name = "Number of matching books")]
        public int NumberOfMatchingBooks { get; set; }

        //Music
        [Display(Name = "Music")]
        public List<MediaItemDTO> Music { get; set; }

        [Display(Name = "Number of matching music files")]
        public int NumberOfMatchingMusic { get; set; }

        //Movies
        [Display(Name = "Movies")]
        public List<MediaItemDTO> Movies { get; set; }

        [Display(Name = "Number of matching movies")]
        public int NumberOfMatchingMovies { get; set; }
    }
}