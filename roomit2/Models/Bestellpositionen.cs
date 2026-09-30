using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace roomit2.Models
{
    public class Bestellpositionen
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Anzahl")]
        public int Anzah { get; set; }

        [DisplayName("Stückpreis")]
        public double Stückpreis { get; set; }

        [DisplayName("Summe")]
        public double Summe { get; set; }

        [DisplayName("Gesamtsumme:")]
        public double Gesamtsumme { get; set; }     

        [DisplayName("Produkt:")]
        public string? ArtikelName { get; set; }

        public string? ProductImage { get; set; }

        public int BestellungId { get; set; }
        public virtual Bestellung Bestellung { get; set; }
    }
}
