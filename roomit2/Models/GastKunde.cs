using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;
using System.Globalization;

namespace roomit2.Models
{
    // Dependent (child)
    public class GastKunde
    {
        public int ID { get; set; }
        public string? KundenType { get; set; }
        public string? Anrede { get; set; }
        public string? Vorname { get; set; }
        public string? Nachname { get; set; }
        public string? Firmaname { get; set; }
        public bool GeschäftsbedingungenAkzeptiert { get; set; }
        public string? Telefon { get; set; }
        public string? Email { get; set; }
        public string? Bestellnummer { get; set; }
        public bool? HasTowAdresse { get; set; }

        public int BestellungID { get; set; } // Required foreign key property
        public Bestellung Bestellung { get; set; } = null; // Required reference navigation to principal

        public string RechStraße { get; set; }
        public string RechPLZ { get; set; }
        public string RechOrt { get; set; }

        public string? LiefStraße { get; set; }
        public string? LiefPLZ { get; set; }
        public string? LiefOrt { get; set; }

        public string ErstellungssDatum
        {
            get { return DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("de-DE")); }
            set { }
        }
        //List<Bestellung> Bestellungen { get; set; } = new List<Bestellung>();
    }
}
