using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace roomit2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? KundenArt { get; set; }
        public string? Anrede { get; set; }

        [Required(ErrorMessage = "Vorname ist ein  Pflichtfeld ")]
        [DataType(DataType.Text)]
        public string? Vorname { get; set; }

        [Required(ErrorMessage = "Nachname ist ein  Pflichtfeld ")]
        [DataType(DataType.Text)]
        public string? Nachname { get; set; }
        public bool GeschäftsbedingungenAkzeptiert { get; set; }
        public bool hasTowAdresse { get; set; }
        public int RechnungAdresseID { get; set; }
        public int LieferAdresseID { get; set; }
        public string? KundenType { get; set; }
        public bool IsOnline { get; set; }

        [NotMapped]
        public RechnungAdresse? RechnungAdresse { get; set; }

        [NotMapped]
        public LieferAdresse? LieferAdresse { get; set; }

        [NotMapped]
        public string Password { get; set; }

    }
}
