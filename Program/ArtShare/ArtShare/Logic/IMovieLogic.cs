using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtShare.Models;

namespace ArtShare.Logic
{
    public interface IMovieLogic
    {
        MovieDetailsModel GetMovieDetailsModel(int id, int? requestingUser);
        bool DeleteMovie(int id, int requestingUser);
        bool EditMovie(MovieDetailsModel model, int requestingUser);
    }
}
