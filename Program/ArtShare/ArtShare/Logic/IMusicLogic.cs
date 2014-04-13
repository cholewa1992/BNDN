using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtShare.Models;

namespace ArtShare.Logic
{
    public interface IMusicLogic
    {
        MusicDetailsModel GetMusicDetailsModel(int id, int? requestingUser);
        bool DeleteMusic(int id, int requestingUser);
        bool EditMusic(MusicDetailsModel model, int requestingUser);
    }
}
