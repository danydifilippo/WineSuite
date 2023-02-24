namespace WineSuite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Prenotazione")]
    public partial class Prenotazione
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prenotazione()
        {
            Rel_Tariffa_Prenotazione = new HashSet<Rel_Tariffa_Prenotazione>();
            Rel_TariffeScelte_Pren = new HashSet<Rel_TariffeScelte_Pren>();
        }

        [Key]
        public int IdPrenotazione { get; set; }

        public int IdUtente { get; set; }

        public int IdEvento { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DataPrenotazione { get; set; }

        public string Note { get; set; }

        public int? TotPaxPrenotate { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? TotDaPagare { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? Sconto { get; set; }
        public int? TotArrivati { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? TotPagContanti { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? TotPagPos { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? TotPagato { get; set; }
        public bool Stato { get; set; }

        public virtual Eventi Eventi { get; set; }

        public virtual Utenti Utenti { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rel_Tariffa_Prenotazione> Rel_Tariffa_Prenotazione { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rel_TariffeScelte_Pren> Rel_TariffeScelte_Pren { get; set; }
    }
}
