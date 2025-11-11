using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class PhienDangNhap
    {
        [Key]
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; }
        public DateTime ThoiGianDangNhap { get; set; } = DateTime.Now;
        public DateTime? ThoiGianHetHan { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
