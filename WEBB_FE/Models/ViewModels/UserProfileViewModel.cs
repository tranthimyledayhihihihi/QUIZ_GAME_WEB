using System.Collections.Generic;
namespace WEBB_FE.Models.ViewModels
{
    public class UserProfileViewModel
    {
        // 1. Thông tin cơ bản (Từ api/user/profile/me)
        public int UserID { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public string AnhDaiDien { get; set; }
        public string VaiTro { get; set; }
        // 2. Chuỗi ngày (Từ api/LichSuChoi/streak)
        public int StreakCount { get; set; } // Số ngày liên tiếp
        // 3. Lịch sử đấu (Từ api/LichSuChoi/my)
        public int TongTranDaChoi { get; set; }
        public List<MatchHistoryItem> LichSuDau { get; set; } = new List<MatchHistoryItem>();
    }
    public class MatchHistoryItem
    {
        public int QuizAttemptID { get; set; }
        public string TenQuiz { get; set; }
        public double Diem { get; set; }
        public int SoCauDung { get; set; }
        public string NgayChoi { get; set; }
    }
}