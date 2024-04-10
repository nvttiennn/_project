using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BELibrary.Entity
{
    [Table("DKTiemChung")]
    public class DKTiemChung
    {
        public int Id { get; set; }
        public int? Vacxin { get; set; }
        public int? MuiThu { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Male { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string CCCD { get; set; }
        public string BHYT { get; set; }
        public string Job { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string Address { get; set; }
        public int? Province { get; set; }
        public int? District { get; set; }
        public string NguoiGH { get; set; }
        public string QuanHe { get; set; }
        public string PhoneGH { get; set; }
        public DateTime? NgayTiem { get; set; }
        public int? BuoiTiem { get; set; }
        public DateTime? NgayDK { get; set; }
        public bool? IsActive { get; set; }
        public string Note { get; set; }
    }
}