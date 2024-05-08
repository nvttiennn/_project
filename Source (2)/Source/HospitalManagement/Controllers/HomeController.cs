using BELibrary.Core.Entity;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var test = dbContext.News.FirstOrDefault();
            //var testview = Mapper.Map<NewsViewModel>(test);

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                ViewBag.Doctors = workScope.Doctors.Include(x => x.Faculty).Take(8).ToList();

                var latestPosts = workScope.Articles.Query(x => !x.IsDelete).OrderByDescending(x => x.ModifiedDate).Take(5).ToList();
                ViewBag.LatestPosts = latestPosts;
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetInfoTiemChung(string sdt, string name)
        {
            try
            {
                HospitalManagementDbContext db = new HospitalManagementDbContext();
                var info = db.DKTiemChungs.Where(x => x.Phone == sdt && x.Name.ToLower().Contains(name.ToLower())).ToList().Select(x => new
                {
                    x.Name,
                    x.CCCD,
                    x.Phone,
                    x.BHYT,
                    x.Address,
                    NgayTiem = (x.NgayTiem ?? DateTime.Now).ToString("dd/MM/yyyy"),
                    BuoiTiem = x.BuoiTiem == 0 ? "Buổi sáng" : "Buôi chiều",
                    NgayDK = (x.NgayDK ?? DateTime.Now).ToString("dd/MM/yyyy")
                }).FirstOrDefault();

                return Json(new
                {
                    status = true,
                    data = info
                });
            }
            catch (Exception)
            {
            }

            return Json(new
            {
                status = false
            });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page. ";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page. ";

            return View();
        }

        public ActionResult E404()
        {
            ViewBag.Message = "Your contact page. ";

            return View();
        }
    }
}