using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WineSuite.Models;

namespace WineSuite.Controllers
{
    public class PrenotazioneController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Prenotazione
        public ActionResult Index()
        {
            var prenotazione = db.Prenotazione.Include(p => p.Eventi).Include(p => p.Utenti);
            return View(prenotazione.ToList());
        }

        // GET: Prenotazione/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prenotazione prenotazione = db.Prenotazione.Find(id);
            if (prenotazione == null)
            {
                return HttpNotFound();
            }
            return View(prenotazione);
        }

        // GET: Prenotazione/BookingForUser
        public ActionResult BookingForUser()
        {
            //string UserName = User.Identity.Name;
            //Utenti u = db.Utenti.Where(x => x.Username == UserName).FirstOrDefault();

            List<Prenotazione> prenotazioni = db.Prenotazione.Where(x => x.IdUtente == 1).OrderByDescending(x=>x.IdPrenotazione).ToList();

            List<Rel_Tariffa_Prenotazione> Lista = new List<Rel_Tariffa_Prenotazione>();
            foreach(var item in prenotazioni)
            {
                List<Rel_Tariffa_Prenotazione> r = db.Rel_Tariffa_Prenotazione.Include(x=>x.Tariffe).Include(x=>x.Prenotazione).
                    Where(x => x.IdPrenotazione == item.IdPrenotazione).OrderByDescending(x => x.Tariffe.Tariffa).ToList();
                Lista.AddRange(r);
            }

            ViewBag.ListaTarPren = Lista;

            return View(prenotazioni);
        }

        // POST: Prenotazione/Create

        [HttpPost]

        public ActionResult Create(FormCollection form)
        {

            //Utenti u = db.Utenti.Where(x => x.Username == form["Utente"].ToString()).FirstOrDefault();
            Prenotazione p = new Prenotazione();

            //IdUtente = u.IdUtente,
            p.IdUtente = 1;
            p.IdEvento = Convert.ToInt32(form["IdEvento"]);
            p.Note = form["Note"].ToString();
            p.TotDaPagare = Convert.ToDecimal(form["TotDaPag"]);
            p.TotPaxPrenotate = Convert.ToInt32(form["TotPren"]);
            p.DataPrenotazione = DateTime.Now;

            db.Prenotazione.Add(p);
            db.SaveChanges();

            if (form["Tar[]"]!=null)
            {
                Array a = form["Tar[]"].ToString().Split(',');
                Array b = form["Pax[]"].ToString().Split(',');
                
                int i = 0;
                
                foreach (var item in a)
                {

                    Rel_Tariffa_Prenotazione r = new Rel_Tariffa_Prenotazione();
                    r.IdPrenotazione = p.IdPrenotazione;
                    r.IdTariffa = Convert.ToInt32(item);
                    r.NrPax = Convert.ToInt32(b.GetValue(i));                   
                    db.Rel_Tariffa_Prenotazione.Add(r);
                    db.SaveChanges();
                    i++;
                }
            }
       
            return RedirectToAction("BookingForUser");

        }

        // GET: Prenotazione/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prenotazione prenotazione = db.Prenotazione.Find(id);
            if (prenotazione == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEvento = new SelectList(db.Eventi, "IdEvento", "Titolo", prenotazione.IdEvento);
            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Ruolo", prenotazione.IdUtente);
            return View(prenotazione);
        }

        // POST: Prenotazione/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPrenotazione,IdUtente,IdEvento,Note,TotPaxPrenotate,TotDaPagare,TotArrivati,TotPagContanti,TotPagPos,TotPagato")] Prenotazione prenotazione)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prenotazione).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdEvento = new SelectList(db.Eventi, "IdEvento", "Titolo", prenotazione.IdEvento);
            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Ruolo", prenotazione.IdUtente);
            return View(prenotazione);
        }


        // POST: Prenotazione/Delete/5
        [HttpPost]

        public ActionResult Delete(int id)
        {
            Prenotazione prenotazione = db.Prenotazione.Find(id);
            var r = db.Rel_Tariffa_Prenotazione.Where(x=>x.IdPrenotazione== prenotazione.IdPrenotazione).ToList();
            foreach(var item in r)
            {
            db.Rel_Tariffa_Prenotazione.Remove(item);
            db.SaveChanges();

            }
            db.Prenotazione.Remove(prenotazione);
            db.SaveChanges();
            return Redirect("/Prenotazione/BookingForUser");
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
