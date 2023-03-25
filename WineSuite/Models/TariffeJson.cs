using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WineSuite.Models
{
    public class TariffeJson
    {
        public int IdTariffa { get; set; }
        public decimal Tariffa { get; set; }

        public string DescrTariffa { get; set; }

        public int NrPax { get; set; }
    }
}