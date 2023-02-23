using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WineSuite.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext1")
        {
        }

        public virtual DbSet<Eventi> Eventi { get; set; }
        public virtual DbSet<Luogo> Luogo { get; set; }
        public virtual DbSet<Prenotazione> Prenotazione { get; set; }
        public virtual DbSet<Tariffe> Tariffe { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }
        public virtual DbSet<Rel_Tariffa_Prenotazione> Rel_Tariffa_Prenotazione { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tariffe>().HasMany(a => a.Eventi)
                                            .WithMany(t => t.Tariffe);

            modelBuilder.Entity<Eventi>()
                .HasMany(e => e.Prenotazione)
                .WithRequired(e => e.Eventi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Eventi>()
                .HasMany(e => e.Tariffe)
                .WithMany(e => e.Eventi)
                .Map(m => m.ToTable("Rel_Tariffa-Evento").MapLeftKey("IdEvento").MapRightKey("IdTariffa"));

            modelBuilder.Entity<Prenotazione>()
                .Property(e => e.TotDaPagare)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Prenotazione>()
                .Property(e => e.TotPagContanti)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Prenotazione>()
                .Property(e => e.TotPagPos)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Prenotazione>()
                .Property(e => e.TotPagato)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Prenotazione>()
                .HasMany(e => e.Rel_Tariffa_Prenotazione)
                .WithRequired(e => e.Prenotazione)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tariffe>()
                .Property(e => e.Tariffa)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Tariffe>()
                .HasMany(e => e.Rel_Tariffa_Prenotazione)
                .WithRequired(e => e.Tariffe)
                //.WillCascadeOnDelete(false);
                .HasForeignKey(e => e.IdTariffa);

            modelBuilder.Entity<Tariffe>()
               .HasMany(e => e.Rel_Tariffa_Prenotazione1)
               .WithOptional(e => e.Tariffe1)
               .HasForeignKey(e => e.Tariffa_Opzionata);

            modelBuilder.Entity<Utenti>()
                .Property(e => e.Contatto)
                .IsFixedLength();

            modelBuilder.Entity<Utenti>()
                .HasMany(e => e.Prenotazione)
                .WithRequired(e => e.Utenti)
                .WillCascadeOnDelete(false);
        }
    }
}
