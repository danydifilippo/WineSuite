using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WineSuite.Models
{
    public partial class Rel_TariffeScelte_Pren
    {
        [Key]
        public int idRel { get; set; }

        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPrenotazione { get; set; }


        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTariffa { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NrPax { get; set; }
       
        public virtual Prenotazione Prenotazione { get; set; }

        public virtual Tariffe Tariffe { get; set; }
    }
}