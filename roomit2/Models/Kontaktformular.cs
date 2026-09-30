using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace roomit2.Models
{
    public class Kontaktformular
    {
        [Required(ErrorMessage = "Anrede ist ein  Pflichtfeld ")]
        [DataType(DataType.Text)]
        public string? Anrede { get; set; }

        [Required(ErrorMessage = "Vorname ist ein  Pflichtfeld ")]
        [DataType(DataType.Text)]
        public string Vorname { get; set; }

        [Required(ErrorMessage = "Nachname ist ein  Pflichtfeld ")]
        [DataType(DataType.Text)]
        public string Nachname { get; set; }

        [Required(ErrorMessage = "E-Mail ist ein  Pflichtfeld ")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string? Telefon { get; set; }

        [Required(ErrorMessage = "Betreff ist ein  Pflichtfeld ")]
        [DataType(DataType.Text)]
        public string Betreff { get; set; }

        [Required(ErrorMessage = "Nachricht ist ein  Pflichtfeld ")]
        [DataType(DataType.Text)]
        public string Nachricht { get; set; }

        //GoogleReCaptcha
        //[Required(ErrorMessage = "Anti-Roboter-Verifizierung ist ein  Pflichtfeld ")]
        public string? GoogleReCaptchaToken { get; set; }

        //Friendlycaptcha
        [Required(ErrorMessage = "Anti-Roboter-Verifizierung")]
        public string? FriendlycaptchaToken { get; set; }

        public string ErstellungssDatum
        {
            get { return DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("de-DE")); }
            set { }
        }
    }
}
