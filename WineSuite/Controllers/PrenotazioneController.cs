using Microsoft.Ajax.Utilities;
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
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.WebPages;
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

        // Gestisci Prenotazione /Lato Admin

        public ActionResult ManageBooking(int? id)
        {
            ModelDbContext db1 = new ModelDbContext();
            var pren = db1.Prenotazione.Where(x=>x.IdEvento== id).ToList();
            foreach (var pn in pren)
            {
                if (pn.Nome == null)
                {
                    pn.Nome = pn.Utenti.Cognome + " " + pn.Utenti.Nome;
                    db1.Entry(pn).State = EntityState.Modified;
                    db1.SaveChanges();
                }
               
            }

            var prenotazioni = db.Prenotazione.OrderBy(x => x.Nome).Where(x => x.IdEvento == id);
            var e = db.Eventi.Find(id);
            ViewBag.Titolo = e.Titolo;
            ViewBag.Id = e.IdEvento;
            List<Rel_Tariffa_Prenotazione> tariffe = new List<Rel_Tariffa_Prenotazione>();
            foreach(var p in prenotazioni)
            {
                if(p.Nome == null)
                {
                    p.Nome = p.Utenti.Cognome + " " + p.Utenti.Nome;                    
                }
                if (p.Sconto != null)
                {
                    if (!p.Sconto.Contains("%") && !p.Sconto.Contains("€"))
                    {
                        p.ScontoEuro = Convert.ToDecimal(p.Sconto);
                    }
                }
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
            return View(prenotazioni.OrderBy(x=>x.Nome).ToList());
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

            if (prenotazione.Sconto!= null) {
                if (!prenotazione.Sconto.Contains("%") && !prenotazione.Sconto.Contains("€"))
                    {
                    prenotazione.ScontoEuro = Convert.ToDecimal(prenotazione.Sconto);
                    }
            }
            return View(prenotazione);
        }

        [HttpPost]
        public ActionResult AddNewPrices(FormCollection form)
        {
            var rel = form["arr[]"].ToString();

            List<TarPrenJson> b = JsonConvert.DeserializeObject<List<TarPrenJson>>(rel);

            int id = 0;
            int pax = 0;
            decimal tot = 0;

            foreach(var item in b)
            {
                Rel_TariffeScelte_Pren r = new Rel_TariffeScelte_Pren();

                id = item.id;
                r.IdPrenotazione = item.id;
                r.IdTariffa = item.Tar;
                r.NrPax = item.nrpax;
                pax += item.nrpax;
                Tariffe t = db.Tariffe.Find(item.Tar);
                tot += (t.Tariffa * item.nrpax);

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

            Prenotazione p = db.Prenotazione.Find(id);
            p.TotDaPagare = tot;
            p.TotPaxPrenotate = pax;

            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();


            return Redirect("/Prenotazione/Index");
        }

        [HttpPost]
        public JsonResult AddNewClient(string nome, string cognome, string email, string contatto)
        {
            Utenti u = new Utenti();
            u.Nome = nome;
            u.Cognome = cognome;
            u.Email = email;
            u.Ruolo = "user";
            u.Contatto = contatto;
            u.DataCreazione = DateTime.Now;

            int index = email.IndexOf('@');
            u.Username = email.Remove(index);
            u.Password = Membership.GeneratePassword(8,0);
            u.Body = "<body text=#29505c><div align=center><br/>Ti diamo il benvenuto nella community Tredaniele...<br/> " +
            "Accedi al sito con le seguenti credenziali:<br/><br/>"+
            "<b>Username: " + u.Username + "<br/>"+
            "<b>Password: " + u.Password + "</div></body>";


            try
            {
                MailMessage mm = new MailMessage();
                mm.From = new MailAddress("danydifilippo@gmail.com");
                mm.To.Add(email);
                mm.Subject = "Tredaniele - Credenziali WineSuite";

                AlternateView av = AlternateView.CreateAlternateViewFromString("<div align=center><img src=cid:mylogo style= height=100 width=100/></div><br/>" + u.Body, null, "text/html");
                LinkedResource logo = new LinkedResource("C:/Users/pc acer/Downloads/img/logo3D.png");
                mm.IsBodyHtml = true;
                logo.ContentId = "mylogo";
                av.LinkedResources.Add(logo);
                mm.AlternateViews.Add(av);
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                NetworkCredential nc = new NetworkCredential("danydifilippo@gmail.com", "zrlzkfqblpvnhvwy");
                smtp.Credentials = nc;

                mm.Body = "<div align=center>" + logo.ContentId + "</div>";

                smtp.Send(mm);

                db.Utenti.Add(u);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
            }

            return Json (u, JsonRequestBehavior.AllowGet);
        }
        // POST: Prenotazione/Edit/Admin

        [HttpPost]
        public ActionResult Edit(Prenotazione prenotazione, string save)
        {

            Prenotazione p = db.Prenotazione.Find(prenotazione.IdPrenotazione);

            p.IdUtente = prenotazione.IdUtente;
            if(prenotazione.Nome != null)
            {
                p.Nome = prenotazione.Nome;
            }

                if (prenotazione.Note != p.Note)
                {
                    p.Note = prenotazione.Note;
                }
                if (p.Sconto != prenotazione.Sconto)
                {
                    p.Sconto = prenotazione.Sconto;
                }

            var c = db.Rel_Tariffa_Prenotazione.Where(x => x.IdPrenotazione == p.IdPrenotazione).ToList();
            var r = db.Rel_TariffeScelte_Pren.Where(x => x.IdPrenotazione == p.IdPrenotazione).ToList();

            if (prenotazione.totpag > 0 && prenotazione.totpag != p.TotDaPagare)
            {
                p.TotDaPagare = prenotazione.totpag;
            }   

                if (save != null)
                {
                    if (r.Count>0)
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
                            db.Rel_TariffeScelte_Pren.Remove(i);
                            db.SaveChanges();
                        }


                    }
               
                }
                else if (save == null)
                {
                    p.Stato = true;
                    var Pax = 0;
                    foreach (var i in r)
                    {
                        Pax += i.NrPax;
                    }
                    p.TotArrivati = Pax;
                    p.TotPagPos = prenotazione.TotPagPos;
                    p.TotPagContanti = prenotazione.TotPagContanti;
                    p.TotPagato = prenotazione.TotPagato;
                    
                }
            

                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                         
                return RedirectToAction("ManageBooking/" + p.IdEvento);
        }


        // GET: Prenotazione/ChangeName/Admin

        public JsonResult ChangeName(string nome)
        {
            var Nominativo = db.Utenti.ToList();

            List<UtentiJson> ClientList = new List<UtentiJson>();
 
            foreach (var item in Nominativo)
            {
                string NomeCompleto = item.Cognome + " " + item.Nome;
                string FullName = item.Nome + " " + item.Cognome;
                if (NomeCompleto.ToUpper() == nome.ToUpper() || FullName.ToUpper() == nome.ToUpper())
                {
                    UtentiJson u = new UtentiJson
                    {
                        IdUtente = item.IdUtente,
                        Nome = item.Nome,
                        Cognome = item.Cognome,
                        Contatto = item.Contatto,
                        Email = item.Email
                    };
                    ClientList.Add(u);
                }
            }

                return Json(ClientList, JsonRequestBehavior.AllowGet);        
           
        }

            // GET: Prenotazione/Edit/Admin

            public JsonResult EditBook(int id)
        {
            List<Rel_Tariffa_Prenotazione> r = db.Rel_Tariffa_Prenotazione.Include(x => x.Tariffe).Include(x => x.Prenotazione).
                    Where(x => x.IdPrenotazione == id).OrderByDescending(x => x.Tariffe.Tariffa).ToList();

            List<TariffeJson> listatj = new List<TariffeJson>();

            foreach(var t in r)
            {
                TariffeJson tj = new TariffeJson
                {
                    IdTariffa = t.IdTariffa,
                    DescrTariffa = t.Tariffe.DescrTariffa,
                    Tariffa = t.Tariffe.Tariffa,
                    NrPax = t.NrPax
                };
                listatj.Add(tj);
            }
           
            return Json(listatj,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEdit(FormCollection form)
        {
            Array a = form["arr[]"].ToString().Split(',');
            int id = Convert.ToInt32(form["id"]);
            string note = form["note"].ToString();
            Prenotazione p = db.Prenotazione.Find(id);
            if(note != "" && p.Note != note)
            {
                p.Note = note;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }

            var tp = db.Rel_Tariffa_Prenotazione.Where(x=>x.IdPrenotazione == id).ToList();

            for(var i = 0; i<a.Length; i++)
            {
                foreach(var item in tp)
                {
                    if ( Convert.ToInt32(a.GetValue(i)) == item.IdTariffa)
                    {
                        i++;
                        if(item.NrPax != Convert.ToInt32(a.GetValue(i)))
                        {
                            db.Rel_Tariffa_Prenotazione.Remove(item);
                            Rel_Tariffa_Prenotazione rtp = new Rel_Tariffa_Prenotazione
                            {
                                IdPrenotazione = id,
                                IdTariffa = item.IdTariffa,
                                NrPax = Convert.ToInt32(a.GetValue(i))
                            };
                            db.Rel_Tariffa_Prenotazione.Add(rtp);
                            db.SaveChanges();
                        };
                        i++;
                    }
                }
            };

            return RedirectToAction("/BookingForUser");
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
