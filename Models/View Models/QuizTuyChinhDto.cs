using System;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// DTO dùng để hiển thị danh sách các Quiz Tuy Chỉnh (đề xuất) của người dùng.
    /// </summary>
    public class QuizTuyChinhDto
    {
        public int QuizTuyChinhID { get; set; }
        public string TenQuiz { get; set; } = null!;
        public string? MoTa { get; set; }
        public DateTime NgayTao { get; set; }

        // ✅ THÔNG TIN UGC
        public string TrangThai { get; set; } = null!; // Pending, Approved, Rejected
        public int SoCauHoi { get; set; } // Tổng số câu hỏi trong đề xuất này
        public DateTime? NgayDuyet { get; set; }
    }
}