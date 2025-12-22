// Models/InputModels/ThanhTuuCreateUpdateModel.cs

using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class ThanhTuuCreateUpdateModel
    {
        // ===============================================
        // THUỘC TÍNH QUẢN LÝ
        // ===============================================

        [Required]
        [MaxLength(100)]
        public string TenThanhTuu { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string MoTa { get; set; } = null!;

        // ✅ BỔ SUNG: AchievementCode (Cần cho logic backend)
        [Required]
        [MaxLength(50)]
        public string AchievementCode { get; set; } = null!; // FIX lỗi biên dịch

        // ✅ BỔ SUNG: BieuTuong (Cần cho giao diện)
        [MaxLength(255)]
        public string? BieuTuong { get; set; } // FIX lỗi biên dịch

        // ===============================================
        // THUỘC TÍNH ĐIỀU KIỆN
        // ===============================================

        [Required]
        [Range(0, 5000)]
        public int DiemThuong { get; set; }

        [Required]
        [MaxLength(50)]
        // Ví dụ: QuizCount, Streak, Score, TopicMastery
        public string LoaiThanhTuu { get; set; } = null!;

        [Required]
        // Giá trị ngưỡng cần thiết để đạt (ví dụ: 100 câu hỏi, chuỗi ngày 7)
        public int GiaTriCanThiet { get; set; }
    }
}