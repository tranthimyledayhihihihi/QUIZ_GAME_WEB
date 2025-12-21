// Models/InputModels/MatchAnswerModel.cs

using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class MatchAnswerModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CauHoiID { get; set; } // ID của câu hỏi người chơi đang trả lời

        [Required]
        public string DapAnDaChon { get; set; } = string.Empty; // Đáp án A, B, C, hoặc D (hoặc nội dung đáp án)

        [Required]
        [Range(0.0, 15.0)] // Giới hạn thời gian 15s/câu
        public double ThoiGianTraLoi { get; set; } // Thời gian tính từ lúc câu hỏi bắt đầu hiển thị (tính bằng giây)
    }
}