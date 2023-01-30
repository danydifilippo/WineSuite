namespace WineSuite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Eventi")]
    public partial class Eventi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Eventi()
        {
            Prenotazione = new HashSet<Prenotazione>();
            Tariffe = new HashSet<Tariffe>();
        }

        [Key]
        public int IdEvento { get; set; }

        [Required]
        [StringLength(50)]
        public string Titolo { get; set; }

        [Required]
        public string Descrizione { get; set; }

        public string SottoDescrizione { get; set; }

        public int? IdLuogo { get; set; }

        [Required]
        [StringLength(40)]
        public string FotoVetrina { get; set; }

        [StringLength(40)]
        public string Foto1 { get; set; }

        [StringLength(40)]
        public string Foto2 { get; set; }

        [StringLength(40)]
        public string Foto3 { get; set; }

        [Column(TypeName = "date")]
        public DateTime Giorno { get; set; }

        public TimeSpan Ora { get; set; }

        public int NrPaxMax { get; set; }

        public bool Pubblico { get; set; }

        public virtual Luogo Luogo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prenotazione> Prenotazione { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tariffe> Tariffe { get; set; }
    }
}
