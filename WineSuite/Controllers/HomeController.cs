using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WineSuite.Models;

namespace WineSuite.Controllers
{
    public class HomeController : Controller
    {
        private ModelDbContext db = new ModelDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string id)
        {
            if (id != null) { 
                ViewBag.Prenota = "Effettua il login o registrati per poter procedere alla prenotazione";
                TempData["id"] = id;
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "Username,Password")] Utenti u)
        {
            var utente = db.Utenti.Where(x => x.Username == u.Username && x.Password == u.Password).FirstOrDefault();
            
            if (utente != null)
            {
                if(TempData["id"] != null)
                {
                    string id = TempData["id"].ToString();
                    FormsAuthentication.SetAuthCookie(u.Username, false);
                    int idE = Convert.ToInt32(id);
                    return Redirect("/Eventi/Details/" + id); 
                }

                FormsAuthentication.SetAuthCookie(u.Username, false);
                return Redirect(FormsAuthentication.DefaultUrl);
            }

            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(Email e)
        {
            MailAddress mittente = new MailAddress(e.Mittente);
            MailAddress destinatario = new MailAddress("danydifilippo@gmail.com");

            MailMessage messaggio= new MailMessage();
            messaggio.Subject = "Inviata dal sito";
            messaggio.Body = e.Messaggio;
            messaggio.From = destinatario;
            messaggio.To.Add(destinatario);

            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.Credentials = new NetworkCredential("danydifilippo@gmail.com", "everytime");

           
            client.Send(messaggio);

            return View();
        }


        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Utenti utente)
        {
            utente.Ruolo = "user";
            utente.DataCreazione = DateTime.Now;
            db.Utenti.Add(utente);
            db.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}