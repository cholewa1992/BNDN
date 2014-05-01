using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ArtShare.Models
{
    public class AccountModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Location { get; set; }

        public List<PurchaseDTO> PurchaseHistory { set; get; }
        public List<PurchaseDTO> UploadHistory { set; get; }

        public class PurchaseDTO
        {
            public int MediaItemId { get; set; }
            public string Title { set; get; }
            public string Thumbnail { set; get; }
        }

    }
}