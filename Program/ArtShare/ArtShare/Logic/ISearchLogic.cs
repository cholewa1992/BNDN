using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtShare.Models;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public interface ISearchLogic
    {
        SearchModel SearchMediaItems(int from, int to, string searchKey);
        SearchModel SearchMediaItemsByType(int from, int to, MediaItemTypeDTO type, string searchKey);
    }
}
