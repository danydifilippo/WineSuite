namespace WineSuite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Rel_Tariffa-Prenotazione")]
    public partial class Rel_Tariffa_Prenotazione
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTariffa { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPrenotazione { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NrPax { get; set; }

        public virtual Prenotazione Prenotazione { get; set; }

        public virtual Tariffe Tariffe { get; set; }
    }
}
