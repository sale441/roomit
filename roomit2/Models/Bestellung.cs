using MessagePack;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Globalization;

namespace roomit2.Models
{
    // Principal (parent)
    public class Bestellung
    {
       
        public int Id { get; set; }
        public int GesamtAnzahlArtikeln{ get; set; }
        public double GesamtBestellungsBetrag { get; set; }

        [DisplayName("Nummer der Bestellung")]
        [PersonalData]
        public string? Bestellnummer { get; set; }

        [DisplayName("Status der Bestellung")]
        [PersonalData]
        [TempData]
        public string? BestellStatus { get; set; }

        [DisplayName("Stadt:")]
        public string? Stadtname { get; set; }

        [DisplayName("PLZ:")]
        public int? Postleitzahl { get; set; }

        [DisplayName("Niederlassung:")]
        public string? Niederlassung { get; set; }

        [DisplayName("Fortlaufende Nummer")]
        [PersonalData]
        public string? FortlaufendeNummer { get; set; }

        [DisplayName("Liefer Datum")]
        [PersonalData]
        public DateTime LieferDatum { get; set; }

        public string IPAdresse { get; set; }

        public string? ApplicationUserID { get; set; }
        public string? KundenType { get; set; }

        public int? GastKundeID { get; set; }
        public GastKunde? GastKunde { get; set; } // Reference navigation to dependent

        public bool? hasTowAdresse { get; set; }

        public List<Bestellpositionen> Positionen { get; set; }

        public string BestellungssDatum
        {
            get { return DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("de-DE")); }
            set { }
        }

        public Bestellung()
        {
            Positionen = new List<Bestellpositionen>();
        }
    }
}
