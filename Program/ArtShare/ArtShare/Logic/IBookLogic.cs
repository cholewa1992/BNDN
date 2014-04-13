using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtShare.Models;

namespace ArtShare.Logic
{
    interface IBookLogic
    {
        BookDetailsModel GetBookDetailsModel(int id, int? requestingUser);
        bool DeleteBook(int id, int requestingUser);
        bool EditBook(BookDetailsModel model, int requestingUser);
    }
}
