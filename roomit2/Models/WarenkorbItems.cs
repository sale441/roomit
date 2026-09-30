using System.ComponentModel;

namespace roomit2.Models
{
    public class WarenkorbItems
    {
        [DisplayName("Artikel:")]
        public string? ProduktName { get; set; }

        [DisplayName("Anzahl")]
        public int Quantity { get; set; }

        [DisplayName("Stückpreis")]
        public double UnitPrice { get; set; }

        [DisplayName("Summe")]
        public double Total { get; set; }

        public string? ProductImage { get; set; }
    }
}
