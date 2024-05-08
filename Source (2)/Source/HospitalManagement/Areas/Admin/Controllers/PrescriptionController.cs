using BELibrary.Core.Entity;
using BELibrary.DbContext;
using BELibrary.Entity;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class PrescriptionController : BaseController
    {
        // GET: Admin/Attachment
        private const string KeyElement = "Đơn thuốc";

        // GET: Admin/Event
        public ActionResult Index(Guid detailRecordId, Guid patientId, Guid doctorId)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.DetailRecordId = detailRecordId;
            ViewBag.patientId = patientId;
            ViewBag.doctorId = doctorId;

            ViewBag.BaseURL = "#";

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var medicines = workScope.Medicines.GetAll().ToList();
                ViewBag.Medicines = new SelectList(medicines, "Id", "Name");

                var listData = workScope.Prescriptions
                    .Include(x => x.DetailPrescription)
                    .Where(x => x.DetailRecordId == detailRecordId)
                    .ToList();

                return View(listData);
            }
        }

        public ActionResult PrintDonThuoc(Guid detailRecordId, Guid patientId, Guid doctorId)
        {
            HospitalManagementDbContext db = new HospitalManagementDbContext();

            LocalReport localReport = new LocalReport();
            string path = Path.Combine(Server.MapPath("/Reports/"), "Report1.rdlc");
            localReport.ReportPath = path;
            localReport.EnableExternalImages = true;

            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.27in</PageWidth>" +
                "  <PageHeight>11.69in</PageHeight>" +
                "  <MarginTop>0.3in</MarginTop>" +
                "  <MarginLeft>0.1in</MarginLeft>" +
                "  <MarginRight>0.1in</MarginRight>" +
                "  <MarginBottom>0.2in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            var detailRecord = db.DetailRecords.Find(detailRecordId);
            var detailRecordDoctor = db.Doctors.Find(doctorId)?.Name;
            var patient = db.Patients.Find(patientId);

            //Param report: can, klText, month, year, dvGiao, dvNhan, khoxuat
            ReportParameter name1 = new ReportParameter("name1", patient?.FullName ?? "");
            ReportParameter diachi = new ReportParameter("diachi", patient?.Address ?? "");
            ReportParameter chandoan = new ReportParameter("chandoan", detailRecord?.DiseaseName ?? "");
            ReportParameter bacsi = new ReportParameter("bacsi", detailRecordDoctor ?? "");
            ReportParameter ngaykham = new ReportParameter("ngaykham", DateTime.Now.ToString("dd/MM/yyyy"));

            localReport.SetParameters(new ReportParameter[] { name1, diachi, chandoan, bacsi, ngaykham});

            localReport.DataSources.Clear();
            var listData = new List<Prescription>();
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var medicines = workScope.Medicines.GetAll().ToList();
                ViewBag.Medicines = new SelectList(medicines, "Id", "Name");

                listData = workScope.Prescriptions
                    .Include(x => x.DetailPrescription)
                    .Where(x => x.DetailRecordId == detailRecordId)
                    .ToList();
            }

            var data = listData.Select(x => x.DetailPrescription).Select((x, i) => new
            {
                Id = i.ToString(),
                MedicineId = db.Medicines.Find(x.MedicineId)?.Name,
                Amount = x.Amount + " " + x.Unit,
                x.Unit,
                x.Note
            }).ToList();
            ReportDataSource rds = new ReportDataSource("DataSet1", data);
            localReport.DataSources.Add(rds);

            renderedBytes = localReport.Render(
                "PDF",
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var detail = workScope.DetailPrescriptions.FirstOrDefault(x => x.Id == id);

                return detail == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            detail.Id,
                            detail.MedicineId,
                            detail.Amount,
                            detail.Unit,
                            detail.Note
                        }
                    });
            }
        }

        public void UpdateMedicineQuantity(int? quantity, Medicine medicine)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var elm = workScope.Medicines.Get(medicine.Id);

                if (elm != null)
                {
                    elm.Quantity = quantity;
                    workScope.Medicines.Put(elm, elm.Id);
                    workScope.Complete();
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DetailPrescription input, Prescription prescription, bool isEdit)
        {
            HospitalManagementDbContext db = new HospitalManagementDbContext();

            //try
            //{
                var medicine = db.Medicines.Find(input.MedicineId);
                if(input.Amount > medicine.Quantity)
                {
                    return Json(new { status = false, mess = "Số lượng thuốc trong kho không đủ!" });
                }

                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    if (isEdit)
                    {
                        var elm = workScope.DetailPrescriptions.Get(input.Id);

                        if (elm != null) //update
                        {
                            var inputTmp = db.DetailPrescriptions.Find(input.Id);
                            var quantityUpdate = inputTmp.Amount > input.Amount ? (medicine.Quantity + inputTmp.Amount - input.Amount) : (medicine.Quantity - (input.Amount - inputTmp.Amount));
                            UpdateMedicineQuantity(quantityUpdate, medicine);

                            elm = input;
                            workScope.DetailPrescriptions.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                    else
                    {
                        UpdateMedicineQuantity(medicine.Quantity - input.Amount, medicine);

                        input.Id = Guid.NewGuid();

                        workScope.DetailPrescriptions.Add(input);
                        workScope.Complete();

                        workScope.Prescriptions.Add(new Prescription
                        {
                            Id = Guid.NewGuid(),
                            DetailRecordId = prescription.DetailRecordId,
                            DetailPrescriptionId = input.Id,
                            CreatedBy = GetCurrentUser().FullName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = GetCurrentUser().FullName
                        });

                        workScope.Complete();

                        return Json(new { status = true, mess = "Thêm thành công " + KeyElement });
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            //}
        }

        [HttpPost]
        public JsonResult Del(Guid id)
        {
            HospitalManagementDbContext db = new HospitalManagementDbContext();

            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.DetailPrescriptions.Get(id);
                    if (elm != null)
                    {
                        var medicine = db.Medicines.Find(elm.MedicineId);
                        UpdateMedicineQuantity(medicine.Quantity + elm.Amount, medicine);

                        workScope.DetailPrescriptions.Remove(elm);
                        //del
                        var prescriptions = workScope.Prescriptions.Query(x => x.DetailPrescriptionId == elm.Id);
                        foreach (var prescription in prescriptions)
                        {
                            workScope.Prescriptions.Remove(prescription);
                        }
                        workScope.Complete();

                        return Json(new { status = true, mess = "Xóa thành công " + KeyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}