using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WineSuite.Models
{
    public class Email
    {
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Mittente { get; set; }

        [Required]
        public string Messaggio { get; set; }
    }
}