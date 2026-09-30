using Microsoft.EntityFrameworkCore;

namespace roomit2.Models
{
    public class RoomitContext:DbContext
    {
        public RoomitContext(DbContextOptions<RoomitContext> options)
            : base(options)
        {
                
        }

        public virtual DbSet<GastKunde> TblGastkunden { get; set; }
        public virtual DbSet<RechnungAdresse> TblRechnungAdressen { get; set; }
        public virtual DbSet<LieferAdresse> TblLieferAdresse { get; set; }
        public virtual DbSet<Bestellung> TblBestellung { get; set; }
        public virtual DbSet<Bestellpositionen> TblBestellpositionen { get; set; }
        public virtual DbSet<Product> Produkte { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Bestellung>()
                .HasOne(e => e.GastKunde)
                .WithOne(e => e.Bestellung)
                .HasForeignKey<Bestellung>(e => e.GastKundeID)
                .IsRequired(false);

            modelBuilder.Entity<GastKunde>()
                .HasOne(e => e.Bestellung)
                .WithOne(e => e.GastKunde)
                .HasForeignKey<GastKunde>(e => e.BestellungID)
                .IsRequired(true);

            //modelBuilder.Entity<GastKunde>()
            //    .HasOne(e => e.LieferAdresse)
            //    .WithOne(e => e.GastKunde)
            //    .HasForeignKey<RechnungAdresse>(e => e.GastKundeID)
            //    .IsRequired(false);
        }

    }
}
