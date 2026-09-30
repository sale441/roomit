using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace roomit2.Models
{
    public class Warenkorb
    {
        [DisplayName("Anzahl")]
        public int Quantity { get; set; }

        [DisplayName("Gesamtsumme:")]
        public double TotalAmount { get; set; }

        [DisplayName("Stadt:")]
        public string? Stadtname { get; set; }

        [DisplayName("PLZ:")]
        public int? Postleitzahl { get; set; }

        [DisplayName("Niederlassung:")]
        public string? NL { get; set; }

        public double Versandkosten { get; set; }

        [DisplayName("Lieferdatum:")]
        public DateTime LieferDatum { get; set; }

        public string ErstellungssDatum
        {
            get { return DateTime.Now.ToString("dddd, dd MMMM yyyy", new CultureInfo("de-DE")); }
            set { }
        }

        public string? Bestellnummer { get; set; }

        public string? IPAdresse { get; set; }

        public string? ApplicationUserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public GastKunde? GastKunde { get; set; }

        public List<WarenkorbItems> WarenkorbItemsListe { get; set; }

        public Warenkorb()
        {
            WarenkorbItemsListe= new List<WarenkorbItems>();
        }

        
    }
}
