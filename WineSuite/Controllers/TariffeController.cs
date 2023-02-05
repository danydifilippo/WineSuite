using Microsoft.Ajax.Utilities;
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
    public class TariffeController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Tariffe
        public ActionResult Index()
        {
            return View(db.Tariffe.ToList());
        }

        public ActionResult AddNewPrice()
        {
            return PartialView("_AddNewPrice");
        }

        // GET: Tariffe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tariffe/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tariffe tariffe)
        {
            if (ModelState.IsValid)
            {
                db.Tariffe.Add(tariffe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tariffe);
        }

        public JsonResult CreateNewPrice(decimal Tariffa, string DescrTariffa)
        {
            Tariffe t = new Tariffe();
            t.Tariffa = Tariffa;
            t.DescrTariffa = DescrTariffa;
            db.Tariffe.Add(t);
            db.SaveChanges();
            return Json(t, JsonRequestBehavior.AllowGet);
        }

        // GET: Tariffe/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tariffe tariffe = db.Tariffe.Find(id);
            if (tariffe == null)
            {
                return HttpNotFound();
            }
            return View(tariffe);
        }

        // POST: Tariffe/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdTariffa,Tariffa,DescrTariffa")] Tariffe tariffe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tariffe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tariffe);
        }


        public ActionResult Delete(int id)
        {
            Tariffe tariffe = db.Tariffe.Find(id);
            db.Tariffe.Remove(tariffe);
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
