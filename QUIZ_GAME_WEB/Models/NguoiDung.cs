using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class NguoiDung
    {
        [Key]
        public int UserID { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public string AnhDaiDien { get; set; }
        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
