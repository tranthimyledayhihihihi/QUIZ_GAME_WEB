namespace QUIZ_GAME_WEB.Models
{
    // ViewModel này dùng để hiển thị câu hỏi trong danh sách Admin
    public class CauHoiAdminViewModel
    {
        public int CauHoiID { get; set; }
        public string? NoiDung { get; set; }
        public string? TenChuDe { get; set; } // Lấy từ bảng ChuDe
        public string? TenDoKho { get; set; } // Lấy từ bảng DoKho
        public string? DapAnDung { get; set; }
    }
}