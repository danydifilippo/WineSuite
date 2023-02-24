using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WineSuite.Models
{
    [Table("Rel_TariffeScelte_Pren")]
    public class Rel_TariffeScelte_Pren
    {

     
        public int IdTariffa { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPrenotazione { get; set; }


        public int NrPax { get; set; }

        public virtual Prenotazione Prenotazione { get; set; }

        public virtual Tariffe Tariffe { get; set; }
    }

}