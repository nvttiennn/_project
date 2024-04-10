using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class VacxinController : Controller
    {
        private HospitalManagementDbContext db = new HospitalManagementDbContext();
        private const string KeyElement = "Vacxin";

        // GET: Admin/Vacxin
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            return View(db.Vacxins.ToList());
        }

        // GET: Admin/Vacxin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vacxin vacxin = db.Vacxins.Find(id);
            if (vacxin == null)
            {
                return HttpNotFound();
            }
            return View(vacxin);
        }

        // GET: Admin/Vacxin/Create
        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;
            return View();
        }

        // POST: Admin/Vacxin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Quantity,Description")] Vacxin vacxin)
        {
            if (ModelState.IsValid)
            {
                db.Vacxins.Add(vacxin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vacxin);
        }

        // GET: Admin/Vacxin/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = KeyElement;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vacxin vacxin = db.Vacxins.Find(id);
            if (vacxin == null)
            {
                return HttpNotFound();
            }
            return View(vacxin);
        }

        // POST: Admin/Vacxin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Quantity,Description")] Vacxin vacxin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vacxin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vacxin);
        }

        // GET: Admin/Vacxin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vacxin vacxin = db.Vacxins.Find(id);
            if (vacxin == null)
            {
                return HttpNotFound();
            }
            return View(vacxin);
        }

        // POST: Admin/Vacxin/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vacxin vacxin = db.Vacxins.Find(id);
            db.Vacxins.Remove(vacxin);
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
