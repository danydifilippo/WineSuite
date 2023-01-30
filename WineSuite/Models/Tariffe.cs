namespace WineSuite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tariffe")]
    public partial class Tariffe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tariffe()
        {
            Rel_Tariffa_Prenotazione = new HashSet<Rel_Tariffa_Prenotazione>();
            Eventi = new HashSet<Eventi>();
        }

        [Key]
        public int IdTariffa { get; set; }

        [Column(TypeName = "money")]
        public decimal Tariffa { get; set; }

        [Required]
        [StringLength(50)]
        public string DescrTariffa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rel_Tariffa_Prenotazione> Rel_Tariffa_Prenotazione { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Eventi> Eventi { get; set; }
    }
}
