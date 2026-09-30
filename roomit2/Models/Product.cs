using System.ComponentModel.DataAnnotations;

namespace roomit2.Models
{
    public class Product
    {

        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? ProductImage { get; set; }
        public int? LocationNr { get; set; }
        public string? Stadtname { get; set; }
        public int? Postleitzahl { get; set; }
        public string? Artikelnummer { get; set; }
        public string? NL { get; set; }
    }
}
