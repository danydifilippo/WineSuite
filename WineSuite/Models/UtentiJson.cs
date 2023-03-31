using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Reflection;

namespace WineSuite.Models
{
    public class UtentiJson
    {
        public int IdUtente { get; set; }

        public string Ruolo { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public string Contatto { get; set; }

        public DateTime DataCreazione { get; set; }
    }
}

