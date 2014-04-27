using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ArtShare.Models
{
    public class DownloadModel
    {
        public Stream Stream { get; set; }
        public string FileExtension { get; set; }
    }
}