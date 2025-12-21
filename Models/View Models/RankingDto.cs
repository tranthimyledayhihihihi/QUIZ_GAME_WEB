using System;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// Data Transfer Object (DTO) dùng để hiển thị thông tin xếp hạng trên Bảng xếp hạng (Leaderboard).
    /// </summary>
    public class RankingDto
    {
        public int UserID { get; set; }
        public string TenHienThi { get; set; } = null!; // Ví dụ: HoTen hoặc TenDangNhap
        public string? AnhDaiDien { get; set; }

        // === Thông tin Xếp hạng (Từ bảng BXH) ===

        public int HangTuan { get; set; } // Hạng theo tuần
        public int DiemTuan { get; set; } // Điểm theo tuần

        public int HangThang { get; set; } // Hạng theo tháng
        public int DiemThang { get; set; } // Điểm theo tháng

        // Có thể thêm trạng thái (Ví dụ: Đang online)
        public bool IsOnline { get; set; }
    }
}