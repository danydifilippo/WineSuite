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
        public int? IdTariffa { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPrenotazione { get; set; }

        public int? NrPax { get; set; }

        public int? Tariffa_Opzionata { get; set; }

        public int? NrPax_Arrivate { get; set; }

        public virtual Prenotazione Prenotazione { get; set; }

        public virtual Tariffe Tariffe { get; set; }

        public virtual Tariffe Tariffe1 { get; set; }
    }
}
