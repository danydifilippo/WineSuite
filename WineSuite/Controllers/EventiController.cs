using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using WineSuite.Models;

namespace WineSuite.Controllers
{
    public class EventiController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Eventi/index Admin
        public ActionResult Index()
        {
            var eventi = db.Eventi.Include(e => e.Luogo).Where(x=>x.Eliminato == false);
            return View(eventi.ToList());
        }

        // GET: Eventi/index User
        public ActionResult Events()
        {
            var eventi = db.Eventi.Include(e => e.Luogo).Where(x => x.Eliminato == false && x.Pubblico== true);

            List<Tariffe_Eventi> tar = new List<Tariffe_Eventi>();

            foreach(var e in eventi)
            {
                var ev = e.Tariffe.OrderByDescending(x => x.Tariffa);
                foreach(var i in ev)
                {
                    Tariffe_Eventi t = new Tariffe_Eventi
                    {
                        IdEvento = e.IdEvento,
                        IdTariffa = i.IdTariffa,
                        Tariffa = i.Tariffa,
                        DescrTariffa = i.DescrTariffa
                    };
                    tar.Add(t);
                }
            }
           ViewBag.prices = tar;
            return View(eventi.ToList());
        }

        // GET: Eventi/index User
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventi evento = db.Eventi.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            ViewBag.Prices = evento.Tariffe.OrderByDescending(x=>x.Tariffa);
            return View(evento);
        }


        // GET: Eventi/Create
        public ActionResult Create()
        {
            ViewBag.IdLuogo = new SelectList(db.Luogo, "IdLuogo", "Descrizione");
            ViewBag.IdTariffe = Tariffe.ListaTariffe;
            return View();
        }

        // POST: Eventi/Create

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            string Vetrina = "emptyImg.jpeg";
            string Foto1 = "emptyImg.jpeg";
            string Foto2 = "emptyImg.jpeg";
            string Foto3 = "emptyImg.jpeg";

            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {                  
                    HttpPostedFileBase file = files[i];

                    var Path = Server.MapPath("/Content/img/" + file.FileName);
                    file.SaveAs(Path);

                    if(i == 0)
                    {
                        Vetrina = file.FileName;
                    } if (i == 1)
                    {
                        Foto1 = file.FileName;
                    }
                    if (i == 2)
                    {
                        Foto2 = file.FileName;
                    }
                    if (i == 3)
                    {
                        Foto3 = file.FileName;
                    }
                }
               
            }

            Eventi e = new Eventi
            {
                Titolo = form["titolo"].ToString(),
                Descrizione = form["descr"].ToString(),
                SottoDescrizione = form["descr2"].ToString(),
                IdLuogo = Convert.ToInt32(form["luogo"]),
                Giorno = Convert.ToDateTime(form["giorno"]),
                Ora = TimeSpan.Parse(form["ora"]),
                Pubblico = Convert.ToBoolean(form["public"]),
                NrPaxMax = Convert.ToInt32(form["pax"]),
                FotoVetrina = Vetrina,
                Foto1 = Foto1,
                Foto2 = Foto2,
                Foto3 = Foto3
            };

            db.Eventi.Add(e);
            db.SaveChanges();

            if (form["Arr[]"] != null) 
            {
                Array a = form["Arr[]"].ToString().Split(',');

                foreach (var item in a)
                {
                    int i = Convert.ToInt32(item);
                    Tariffe t = db.Tariffe.Find(i);
                    e.Tariffe.Add(t);
                    db.SaveChanges();
                }

            }


            return Redirect("/Eventi/Index");

        }

        // chiamata Ajax: Cambia proprietà booleana Pubblico
        public ActionResult ChangeVisibility(int id)
        {
            Convert.ToInt32(id);
            Eventi evento = db.Eventi.Find(id);
            if(evento.Pubblico == false) 
            {
                evento.Pubblico = true;
            }else
            {
                evento.Pubblico = false;
            }

            db.Entry(evento).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("/Eventi/Index");
        }

        // chiamata Ajax: Cambia proprietà booleana Eliminato
        public ActionResult DeleteVisibility(string id)
        {
            int ID = Convert.ToInt32(id);
            Eventi evento = db.Eventi.Find(ID);
            evento.Eliminato = true;

            db.Entry(evento).State = EntityState.Modified;
            db.SaveChanges();

            RedirectToAction("Index");

            return RedirectToAction("Index");
        }

        // Chiamata Ajax : Delete Tariffa

        public ActionResult DeletePrice(string id, string idEvento)
        {
            int IdPrice = Convert.ToInt32(id);
            int IdEv = Convert.ToInt32(idEvento);
            Eventi evento = db.Eventi.Find(IdEv);
            Tariffe tariffa = db.Tariffe.Find(IdPrice);
            evento.Tariffe.Remove(tariffa);
            db.SaveChanges();
            return Redirect("Eventi/Edit");
        }

        // GET: Eventi/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventi eventi = db.Eventi.Find(id);
            if (eventi == null)
            {
                return HttpNotFound();
            }
         
            ViewBag.IdLuogo = new SelectList(db.Luogo, "IdLuogo", "Descrizione", eventi.IdLuogo);
            ViewBag.IdTariffe = Tariffe.ListaTariffe;
            ViewBag.Prices = eventi.Tariffe;
            return View(eventi);
        }

        // POST: Eventi/Edit/5
        
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            var id = Convert.ToInt32(form["IdEvento"]);
            Eventi ev = db.Eventi.Find(id);
            ev.Titolo = form["titolo"].ToString();
            ev.Descrizione = form["descr"].ToString();
            ev.SottoDescrizione = form["descr2"].ToString();
            ev.IdLuogo = Convert.ToInt32(form["luogo"]);
            ev.Giorno = Convert.ToDateTime(form["giorno"]);
            ev.Ora = TimeSpan.Parse(form["ora"]);
            ev.Pubblico = Convert.ToBoolean(form["public"]);
            ev.NrPaxMax = Convert.ToInt32(form["pax"]);
            var fv = form["FotoVetrina"].ToString().Replace("C:\\fakepath\\", "");
            var f1 = form["Foto1"].ToString().Replace("C:\\fakepath\\","");
            var f2 = form["Foto2"].ToString().Replace("C:\\fakepath\\", "");
            var f3 = form["Foto3"].ToString().Replace("C:\\fakepath\\", "");

            if (fv != "")
            {
                ev.FotoVetrina = fv; 
            }

            if (f1 != "")
            {
                ev.Foto1 = f1;
            }

            if (f2 != "")
            {
                ev.Foto2 = f2;
            }

            if (f3 != "")
            {
                ev.Foto3 = f3;
            }

            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    var Path = Server.MapPath("/Content/img/" + file.FileName);
                    file.SaveAs(Path);

                }
            }

            db.Entry(ev).State = EntityState.Modified;
            db.SaveChanges();

            if (form["Arr[]"] != null)
            {
                Array a = form["Arr[]"].ToString().Split(',');

                foreach (var item in a)
                    {
                        int i = Convert.ToInt32(item);
                        Tariffe t = db.Tariffe.Find(i);
                        ev.Tariffe.Add(t);
                        db.SaveChanges();
                    }
            }

            return Redirect("/Eventi/Index");
        }

      
        // Json: DeleteEvent / il post è il DeleteVisibility
        public JsonResult DeleteEvent(string id)
        {
            int Id = Convert.ToInt32(id);
            Eventi eventi = db.Eventi.Find(Id);
            EventiJson ev = new EventiJson
            {
                Titolo = eventi.Titolo,
            };
            
            return Json(ev, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
