using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Objects.DataClasses;
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
        [Authorize(Roles ="admin")]
        public ActionResult Index()
        {
            var eventi = db.Eventi.ToList();
            foreach(var e in eventi)
            {
                e.TotPrenotazioni = db.Prenotazione.GroupBy(x=>x.IdEvento == e.IdEvento).Count() ;
                e.TotPaxPrenotate = db.Prenotazione.Where(x => x.IdEvento == e.IdEvento).Sum(x => x.TotPaxPrenotate);
                e.TotPaxArrivate = db.Prenotazione.Where(x => x.IdEvento == e.IdEvento).Sum(x => x.TotArrivati);
                if (e.TotPaxArrivate == null)
                {
                    e.TotPaxArrivate = 0;
                }
                e.TotPagato = db.Prenotazione.Where(x => x.IdEvento == e.IdEvento).Sum(x => x.TotPagato);
                if (e.TotPagato == null)
                {
                    e.TotPagato = 0;
                }
                e.TotContanti = db.Prenotazione.Where(x => x.IdEvento == e.IdEvento).Sum(x => x.TotPagContanti);
                e.TotPos = db.Prenotazione.Where(x => x.IdEvento == e.IdEvento).Sum(x => x.TotPagPos);
                if (e.TotContanti == null)
                {
                    e.TotContanti = 0;
                }
                if (e.TotPos == null)
                {
                    e.TotPos = 0;
                }
            }

            return View(eventi);
        }

        public ActionResult ManageBooking(int? id)
        {
            var prenotazioni = db.Prenotazione.Include(x=>x.Eventi).Include(x=>x.Utenti).Where(x=>x.IdEvento== id).OrderBy(x=>x.Utenti.Cognome);
            var e = db.Eventi.Find(id);
            ViewBag.Titolo = e.Titolo;
            ViewBag.Id = e.IdEvento;
            List<Rel_Tariffa_Prenotazione> tariffe = new List<Rel_Tariffa_Prenotazione>();
            foreach(var p in prenotazioni)
            {
                List<Rel_Tariffa_Prenotazione> r = db.Rel_Tariffa_Prenotazione.Include(x => x.Tariffe).Include(x => x.Prenotazione).
                   Where(x => x.IdPrenotazione == p.IdPrenotazione).OrderByDescending(x => x.Tariffe.Tariffa).ToList();
                tariffe.AddRange(r);
            }
            List<Rel_TariffeScelte_Pren> tariffeScelte = new List<Rel_TariffeScelte_Pren>();
            foreach (var pr in prenotazioni)
            {
                List<Rel_TariffeScelte_Pren> t = db.Rel_TariffeScelte_Pren.Include(x => x.Tariffe).Include(x => x.Prenotazione).
                   Where(x => x.IdPrenotazione == pr.IdPrenotazione).OrderByDescending(x => x.Tariffe.Tariffa).ToList();
                tariffeScelte.AddRange(t);
            }
            ViewBag.Tariffe = tariffe.ToList();
            ViewBag.TariffeScelte = tariffeScelte.ToList();
            return View(prenotazioni.ToList());
        }

        [Authorize]
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
            if (User.Identity.IsAuthenticated) { 
            string UserName = User.Identity.Name;
            Utenti u = db.Utenti.Where(x => x.Username == UserName).FirstOrDefault();

            List<Prenotazione> prenotazioni = db.Prenotazione.Where(x => x.IdUtente == u.IdUtente).OrderByDescending(x=>x.IdPrenotazione).ToList();

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
            else
            {
                return View();
            }
        }

        // POST: Prenotazione/Create

        [HttpPost]

        public ActionResult Create(FormCollection form)
        {
            var user = form["Utente"].ToString();
            Utenti u = db.Utenti.Where(x => x.Username == user).FirstOrDefault();
            Prenotazione p = new Prenotazione();

            p.IdUtente = u.IdUtente;          
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
            Prenotazione prenotazione = db.Prenotazione.Include(x=>x.Rel_TariffeScelte_Pren).Where(x=>x.IdPrenotazione == id).FirstOrDefault();
            if (prenotazione == null)
            {
                return HttpNotFound();
            }
            
            List<Rel_Tariffa_Prenotazione> Lista = new List<Rel_Tariffa_Prenotazione>();
                List<Rel_Tariffa_Prenotazione> r = db.Rel_Tariffa_Prenotazione.Include(x => x.Tariffe).Include(x => x.Prenotazione).
                    Where(x => x.IdPrenotazione == prenotazione.IdPrenotazione).OrderByDescending(x => x.Tariffe.Tariffa).ToList();
                Lista.AddRange(r);

            var idE = prenotazione.IdEvento;
            Eventi evento = db.Eventi.Find(idE);


            ViewBag.ListaTarPren = Lista;
            ViewBag.ListaTariffe = evento.Tariffe ;


            return View(prenotazione);
        }

        [HttpPost]
        public ActionResult AddNewPrices(FormCollection form)
        {
            var rel = form["arr[]"].ToString();

            List<TarPrenJson> b = JsonConvert.DeserializeObject<List<TarPrenJson>>(rel);


            foreach(var item in b)
            {
                Rel_TariffeScelte_Pren r = new Rel_TariffeScelte_Pren();

                r.IdPrenotazione = item.id;
                r.IdTariffa = item.Tar;
                r.NrPax = item.nrpax;

                var newtar = db.Rel_TariffeScelte_Pren.Where(x => x.IdTariffa == item.Tar && x.IdPrenotazione == item.id).FirstOrDefault();
                if (newtar == null)
                {
                db.Rel_TariffeScelte_Pren.Add(r);
                db.SaveChanges();
                }
                else
                {
                    newtar.NrPax = item.nrpax;
                    db.Entry(newtar).State = EntityState.Modified;
                    db.SaveChanges();
                }
            };

            return Redirect("/Prenotazione/Index");
        }


        // POST: Prenotazione/Edit/5

        [HttpPost]
        public ActionResult Edit(Prenotazione prenotazione, string save)
        {

                Prenotazione p = db.Prenotazione.Find(prenotazione.IdPrenotazione);
                if (p.Note != prenotazione.Note)
                {
                    p.Note = prenotazione.Note;
                }
                if (p.Sconto != prenotazione.Sconto)
                {
                    p.Sconto = prenotazione.Sconto;
                }

            var c = db.Rel_Tariffa_Prenotazione.Where(x => x.IdPrenotazione == p.IdPrenotazione).ToList();
            var r = db.Rel_TariffeScelte_Pren.Where(x => x.IdPrenotazione == p.IdPrenotazione).ToList();

            if (prenotazione.totpag != p.TotDaPagare)
            {
                p.TotDaPagare = prenotazione.totpag;
                

                if (save != null)
                {
                    if (r != null)
                    {
                        foreach (var item in c)
                        {
                            db.Rel_Tariffa_Prenotazione.Remove(item);
                            db.SaveChanges();
                        }

                        p.TotPaxPrenotate = 0;
                        foreach (var i in r)
                        {
                            Rel_Tariffa_Prenotazione t = new Rel_Tariffa_Prenotazione();

                            t.IdPrenotazione = i.IdPrenotazione;
                            t.IdTariffa = i.IdTariffa;
                            t.NrPax = i.NrPax;
                            p.TotPaxPrenotate += i.NrPax;
                            db.Rel_Tariffa_Prenotazione.Add(t);
                            db.SaveChanges();
                        }
                        db.Entry(p).State = EntityState.Modified;
                        db.SaveChanges();

                    }
               
                }

            }
            if (save == null)
            {
                p.Stato = true;
                foreach (var i in r)
                {                  
                    p.TotArrivati += i.NrPax;
                }
                p.TotPagPos = prenotazione.TotPagPos;
                p.TotPagContanti = prenotazione.TotPagContanti;
                p.TotPagato = prenotazione.TotPagato;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }

               
                return Redirect("/Prenotazione/ManageBooking/" + p.IdEvento);
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
