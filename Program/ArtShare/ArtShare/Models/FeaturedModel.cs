using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtShare.Models
{
    public class FeaturedModel
    {
        public List<MovieDetailsModel> Movies { get; set; }

        public List<MusicDetailsModel> Music { get; set; }

        public List<BookDetailsModel> Books { get; set; }
    }
}