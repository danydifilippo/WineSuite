namespace WineSuite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using System.Web.WebPages.Html;
    using SelectListItem = System.Web.Mvc.SelectListItem;

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
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Tariffa { get; set; }

        [Required]
        [StringLength(50)]
        public string DescrTariffa { get; set; }

        public static List<SelectListItem> ListaTariffe
        {
            get
            {
                List<SelectListItem> lista = new List<SelectListItem>();
                ModelDbContext db = new ModelDbContext();
                List<Tariffe> t = db.Tariffe.ToList();
                foreach (var item in t)
                {
                    SelectListItem s = new SelectListItem
                    {
                        Text = item.Tariffa.ToString("c2") + " - " + item.DescrTariffa,
                        Value = item.IdTariffa.ToString(),
                    };
                    lista.Add(s);
                }
                return lista;

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rel_Tariffa_Prenotazione> Rel_Tariffa_Prenotazione { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Eventi> Eventi { get; set; }
    }
}
