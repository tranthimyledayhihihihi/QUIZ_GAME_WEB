using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    /// <summary>
    /// Entity đại diện cho Độ khó của Câu hỏi (ví dụ: Dễ, Trung bình, Khó).
    /// </summary>
    public class DoKho
    {
        // ===============================================
        // KHÓA CHÍNH (PRIMARY KEY) VÀ IDENTITY
        // ===============================================

        [Key]
        /// <summary>
        /// Báo cho Entity Framework biết rằng cột này được tạo tự động bởi Database (SQL Identity).
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoKhoID { get; set; }

        // ===============================================
        // THUỘC TÍNH DỮ LIỆU
        // ===============================================

        [Required]
        [MaxLength(50)]
        public string TenDoKho { get; set; } = null!;

        /// <summary>
        /// Điểm thưởng nhận được khi trả lời đúng một câu hỏi thuộc độ khó này.
        /// </summary>
        public int DiemThuong { get; set; } = 0;

        // ===============================================
        // THUỘC TÍNH ĐIỀU HƯỚNG (NAVIGATION PROPERTIES)
        // ===============================================

        /// <summary>
        /// Danh sách các Câu hỏi (CauHoi) thuộc độ khó này.
        /// </summary>
        [JsonIgnore] // Bắt buộc: Ngăn ngừa vòng lặp tham chiếu JSON (JSON Circular Reference)
        public virtual ICollection<CauHoi> CauHois { get; set; } = new HashSet<CauHoi>();
    }
}