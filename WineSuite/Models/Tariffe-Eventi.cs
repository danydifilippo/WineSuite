using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WineSuite.Models
{
    public class Tariffe_Eventi
    {
        [NotMapped]
        public int IdEvento { get; set; }

        [NotMapped]
        public int IdTariffa { get; set; }

        [NotMapped]
        public decimal Tariffa { get; set; }

        [NotMapped]
        public string DescrTariffa { get; set; }
    }
}