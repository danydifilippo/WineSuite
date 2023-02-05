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
    public class LuogoController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Luogo
        public ActionResult Index()
        {
            return View(db.Luogo.ToList());
        }


        // GET: Luogo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Luogo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Luogo luogo)
        {
            if (ModelState.IsValid)
            {
                db.Luogo.Add(luogo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(luogo);
        }

        // GET: Luogo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Luogo luogo = db.Luogo.Find(id);
            if (luogo == null)
            {
                return HttpNotFound();
            }
            return View(luogo);
        }

        // POST: Luogo/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdLuogo,Descrizione,Indirizzo")] Luogo luogo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(luogo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(luogo);
        }

        // GET: Luogo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Luogo luogo = db.Luogo.Find(id);
            if (luogo == null)
            {
                return HttpNotFound();
            }
            return View(luogo);
        }

        // POST: Luogo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Luogo luogo = db.Luogo.Find(id);
            db.Luogo.Remove(luogo);
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
