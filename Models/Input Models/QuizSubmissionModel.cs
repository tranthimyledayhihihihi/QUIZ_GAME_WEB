using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    /// <summary>
    /// Model chứa dữ liệu người dùng gửi lên để đề xuất một Quiz mới (UGC).
    /// </summary>
    public class QuizSubmissionModel
    {
        [Required(ErrorMessage = "Tên Quiz là bắt buộc.")]
        [MaxLength(100)]
        public string TenQuiz { get; set; } = null!;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        // Danh sách các câu hỏi mà người dùng đề xuất
        [Required(ErrorMessage = "Phải có ít nhất một câu hỏi trong đề xuất.")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một câu hỏi trong đề xuất.")]
        public List<CauHoiCreateModel> Questions { get; set; } = new List<CauHoiCreateModel>();
    }

    /// <summary>
    /// Model dùng để tạo một CauHoi mới trong đề xuất (submission).
    /// </summary>
    public class CauHoiCreateModel
    {
        [Required]
        public int ChuDeID { get; set; }

        [Required]
        public int DoKhoID { get; set; }

        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc.")]
        [MaxLength(500)]
        public string NoiDung { get; set; } = null!;

        [Required]
        public string DapAnA { get; set; } = null!;
        [Required]
        public string DapAnB { get; set; } = null!;
        [Required]
        public string DapAnC { get; set; } = null!;
        [Required]
        public string DapAnD { get; set; } = null!;

        [Required(ErrorMessage = "Đáp án đúng (A, B, C, hoặc D) là bắt buộc.")]
        [MaxLength(10)]
        public string DapAnDung { get; set; } = null!;

        [MaxLength(255)]
        public string? HinhAnh { get; set; }
    }
}