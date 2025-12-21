using System;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// DTO dùng để hiển thị danh sách các bản ghi chia sẻ Quiz (đã gửi hoặc đã nhận).
    /// </summary>
    public class QuizShareDto
    {
        public int QuizChiaSeID { get; set; }
        public int QuizTuyChinhID { get; set; }
        public string TenQuiz { get; set; } = null!;
        public DateTime NgayChiaSe { get; set; }

        // Thông tin người gửi (dùng cho list Received)
        public int UserGuiID { get; set; }
        public string TenNguoiGui { get; set; } = null!;

        // Thông tin người nhận (dùng cho list Sent)
        public int UserNhanID { get; set; }
        public string TenNguoiNhan { get; set; } = null!;
    }
}