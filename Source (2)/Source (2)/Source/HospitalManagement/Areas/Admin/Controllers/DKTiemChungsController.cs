using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class DKTiemChungsController : BaseController
    {
        private HospitalManagementDbContext db = new HospitalManagementDbContext();
        private const string KeyElement = "Đăng ký tiêm chủng";

        // GET: Admin/DKTiemChungs
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            return View(db.DKTiemChungs.ToList());
        }

        // GET: Admin/DKTiemChungs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKTiemChung dKTiemChung = db.DKTiemChungs.Find(id);
            if (dKTiemChung == null)
            {
                return HttpNotFound();
            }
            return View(dKTiemChung);
        }

        // GET: Admin/DKTiemChungs/Create
        public ActionResult Create()
        {
            return RedirectToAction("Edit", new { id = 0 });
            //return View();
        }

        // POST: Admin/DKTiemChungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MuiThu,Birthday,Male,Phone,CCCD,BHYT,Job,DanToc,QuocTich,Address,Province,District,NguoiGH,QuanHe,PhoneGH,NgayTiem,BuoiTiem,NgayDK")] DKTiemChung dKTiemChung)
        {
            if (ModelState.IsValid)
            {
                db.DKTiemChungs.Add(dKTiemChung);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dKTiemChung);
        }

        // GET: Admin/DKTiemChungs/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Feature = "Thông tin đăng ký";
            ViewBag.Element = KeyElement;

            if (id == null)
            {
                var dKTiemChungNew = new DKTiemChung();
                return View(dKTiemChungNew);
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKTiemChung dKTiemChung = db.DKTiemChungs.Find(id);
            if (dKTiemChung == null)
            {
                dKTiemChung = new DKTiemChung();
                return View(dKTiemChung);
                //return HttpNotFound();
            }
            return View(dKTiemChung);
        }

        // POST: Admin/DKTiemChungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MuiThu,Birthday,Male,Phone,CCCD,BHYT,Job,DanToc,QuocTich,Address,Province,District,NguoiGH,QuanHe,PhoneGH,NgayTiem,BuoiTiem,NgayDK,Name,IsActive,Vacxin")] DKTiemChung dKTiemChung)
        {
            var tmpDKTiemChung = db.DKTiemChungs.Where(x => x.CCCD == dKTiemChung.CCCD && x.IsActive == true).FirstOrDefault();
            if (tmpDKTiemChung == null)
            {
                return View(dKTiemChung);
            }

            try
            {
                if (dKTiemChung.Id != 0)
                {
                    var vacxin = db.Vacxins.Find(dKTiemChung.Vacxin);
                    if (vacxin != null)
                    {
                        if (tmpDKTiemChung.IsActive != true && dKTiemChung.IsActive == true)
                        {
                            vacxin.Quantity = vacxin.Quantity - 1;
                        }
                        else if (tmpDKTiemChung.IsActive == true && dKTiemChung.IsActive != true)
                        {
                            vacxin.Quantity = vacxin.Quantity + 1;
                        }

                        db.Entry(vacxin).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    dKTiemChung.NgayDK = DateTime.Now;
                    db.Entry(dKTiemChung).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    dKTiemChung.NgayDK = DateTime.Now;
                    db.DKTiemChungs.Add(dKTiemChung);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
            }

            return View(dKTiemChung);
        }

        [HttpPost]
        public JsonResult EditAjax(int? IsActive, [Bind(Include = "Id,MuiThu,Birthday,Male,Phone,CCCD,BHYT,Job,DanToc,QuocTich,Address,Province,District,NguoiGH,QuanHe,PhoneGH,NgayTiem,BuoiTiem,NgayDK,Name,IsActive,Vacxin,Note")] DKTiemChung dKTiemChung)
        {
            try
            {
                try
                {
                    dKTiemChung.IsActive = IsActive == null ? false : true;
                    if (dKTiemChung.Id != 0)
                    {
                        var tmpDKTiemChung = db.DKTiemChungs.Where(x => x.Id == dKTiemChung.Id).FirstOrDefault();
                        var vacxin = db.Vacxins.Find(dKTiemChung.Vacxin);
                        if (vacxin != null)
                        {
                            if (tmpDKTiemChung.IsActive != true && dKTiemChung.IsActive == true)
                            {
                                vacxin.Quantity = vacxin.Quantity - 1;
                            }
                            else if (tmpDKTiemChung.IsActive == true && dKTiemChung.IsActive != true)
                            {
                                vacxin.Quantity = vacxin.Quantity + 1;
                            }

                            db.Entry(vacxin).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        tmpDKTiemChung.IsActive = dKTiemChung.IsActive;
                        tmpDKTiemChung.Birthday = dKTiemChung.Birthday;
                        tmpDKTiemChung.Male = dKTiemChung.Male;
                        tmpDKTiemChung.Phone = dKTiemChung.Phone;
                        tmpDKTiemChung.CCCD = dKTiemChung.CCCD;
                        tmpDKTiemChung.BHYT = dKTiemChung.BHYT;
                        tmpDKTiemChung.Job = dKTiemChung.Job;
                        tmpDKTiemChung.DanToc = dKTiemChung.DanToc;
                        tmpDKTiemChung.QuocTich = dKTiemChung.QuocTich;
                        tmpDKTiemChung.Address = dKTiemChung.Address;
                        tmpDKTiemChung.Province = dKTiemChung.Province;
                        tmpDKTiemChung.District = dKTiemChung.District;
                        tmpDKTiemChung.NguoiGH = dKTiemChung.NguoiGH;
                        tmpDKTiemChung.QuanHe = dKTiemChung.QuanHe;
                        tmpDKTiemChung.PhoneGH = dKTiemChung.PhoneGH;
                        tmpDKTiemChung.NgayTiem = dKTiemChung.NgayTiem;
                        tmpDKTiemChung.BuoiTiem = dKTiemChung.BuoiTiem;
                        tmpDKTiemChung.NgayDK = dKTiemChung.NgayDK;
                        tmpDKTiemChung.Name = dKTiemChung.Name;
                        tmpDKTiemChung.Note = dKTiemChung.Note;
                        db.Entry(tmpDKTiemChung).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { status = true, mess = "Cập nhật thành công." });
                    }
                    else
                    {
                        dKTiemChung.NgayDK = DateTime.Now;
                        db.DKTiemChungs.Add(dKTiemChung);
                        db.SaveChanges();
                        return Json(new { status = true, mess = "Thêm mới thành công." });
                    }
                }
                catch (Exception)
                {
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
            return Json(new { status = false, mess = "Thất bại" });
        }

        // GET: Admin/DKTiemChungs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKTiemChung dKTiemChung = db.DKTiemChungs.Find(id);
            if (dKTiemChung == null)
            {
                return HttpNotFound();
            }
            return View(dKTiemChung);
        }

        // POST: Admin/DKTiemChungs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DKTiemChung dKTiemChung = db.DKTiemChungs.Find(id);
            db.DKTiemChungs.Remove(dKTiemChung);
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
