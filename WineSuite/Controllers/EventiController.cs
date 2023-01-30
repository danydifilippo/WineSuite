using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            return View();
        }

        // POST: Eventi/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdEvento,Titolo,Descrizione,SottoDescrizione,IdLuogo,FotoVetrina,Foto1,Foto2,Foto3,Giorno,Ora,NrPaxMax")] Eventi eventi)
        {
            if (ModelState.IsValid)
            {
                db.Eventi.Add(eventi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdLuogo = new SelectList(db.Luogo, "IdLuogo", "Descrizione", eventi.IdLuogo);
            return View(eventi);
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
            return View(eventi);
        }

        // POST: Eventi/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdEvento,Titolo,Descrizione,SottoDescrizione,IdLuogo,FotoVetrina,Foto1,Foto2,Foto3,Giorno,Ora,NrPaxMax")] Eventi eventi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdLuogo = new SelectList(db.Luogo, "IdLuogo", "Descrizione", eventi.IdLuogo);
            return View(eventi);
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
