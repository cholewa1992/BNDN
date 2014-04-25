using System.ComponentModel.DataAnnotations;

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

    }
}