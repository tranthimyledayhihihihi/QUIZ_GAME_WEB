// Models/InputModels/AnswerSubmitModel.cs
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class AnswerSubmitModel
    {
        // ID phiên làm bài (bắt buộc)
        [Required(ErrorMessage = "ID phiên làm bài là bắt buộc.")]
        public int QuizAttemptID { get; set; }

        // ID câu hỏi
        [Required(ErrorMessage = "Câu hỏi là bắt buộc.")]
        public int CauHoiID { get; set; }

        // Đáp án người chơi đã chọn
        [Required(ErrorMessage = "Đáp án đã chọn là bắt buộc.")]
        [StringLength(10)]
        public string DapAnDaChon { get; set; } = null!; // Ví dụ: "A", "B", "C", "D"

        // ID người chơi (UserID) – quan trọng để lưu vào CauSai và KetQua
        [Required(ErrorMessage = "UserID là bắt buộc.")]
        public int UserID { get; set; }
    }
}
