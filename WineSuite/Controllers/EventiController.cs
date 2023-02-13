using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

        // GET: Eventi
        public ActionResult Index()
        {
            var eventi = db.Eventi.Include(e => e.Luogo);
            return View(eventi.ToList());
        }

        // GET: Eventi/Details/5
        public ActionResult Details(int? id)
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
            return View(eventi);
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

        // JSON : Delete Tariffa

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
            var fv = form["FotoVetrina"].ToString();
            var f1 = form["Foto1"].ToString();
            var f2 = form["Foto2"].ToString();
            var f3 = form["Foto3"].ToString();



            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[0];

                    //if (Vetrina == file.FileName)
                    //{
                        
                    //    var Path = Server.MapPath("/Content/img/" + file.FileName);
                    //    file.SaveAs(Path);
                    //    Vetrina = file.FileName;
                    //}
                    //if (i == 1 && Foto1 != file.FileName)
                    //{
                    //    var Path = Server.MapPath("/Content/img/" + file.FileName);
                    //    file.SaveAs(Path);
                    //    Foto1 = file.FileName;
                    //}
                    //if (i == 2 && Foto2 != file.FileName)
                    //{
                    //    var Path = Server.MapPath("/Content/img/" + file.FileName);
                    //    file.SaveAs(Path);
                    //    Foto2 = file.FileName;
                    //}
                    //if (i == 3 && Foto3 != file.FileName)
                    //{
                    //    var Path = Server.MapPath("/Content/img/" + file.FileName);
                    //    file.SaveAs(Path);
                    //    Foto3 = file.FileName;
                    //}
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

      
        // GET: Eventi/Delete/5
        public ActionResult Delete(int? id)
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
            return View(eventi);
        }

        // POST: Eventi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Eventi eventi = db.Eventi.Find(id);
            db.Eventi.Remove(eventi);
            db.SaveChanges();
            return RedirectToAction("Index");
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
