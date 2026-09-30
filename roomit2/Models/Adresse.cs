using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace roomit2.Models
{
    public class Adresse
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Straße ist ein  Pflichtfeld")]
        [DataType(DataType.Text, ErrorMessage = "Keine richtige Format")]
        public string? Straße { get; set; }

        [Required(ErrorMessage = "PLZ ist ein  Pflichtfeld")]
        [DataType(DataType.PostalCode,ErrorMessage = "Keine richtige Format")]
        public string? PLZ { get; set; }

        [Required(ErrorMessage = "Ort ist ein  Pflichtfeld")]
        [DataType(DataType.Text, ErrorMessage = "Keine richtige Format")]
        public string? Ort { get; set; }
        public string? AdresseTyp { get; set; }
        public string? ApplicationUserID { get; set; }

        public string ErstellungssDatum
        {
            get { return DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("de-DE")); }
            set { }
        }
    }


    public class RechnungAdresse : Adresse
    {

    }
    public class LieferAdresse : Adresse 
    { 

    }

}
