using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Text.Json.Serialization; // Cần thiết để ngăn lỗi vòng lặp JSON

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class QuizTuyChinh
    {
        [Key]
        public int QuizTuyChinhID { get; set; }

        [Required]
        public int UserID { get; set; } // Người tạo/đóng góp

        [Required]
        [MaxLength(100)]
        public string TenQuiz { get; set; } = null!;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // ===============================================
        // ✅ BỔ SUNG TRƯỜNG UGC VÀ PHÊ DUYỆT
        // (Cần thiết cho luồng Admin duyệt đề xuất của người dùng)
        // ===============================================

        /// <summary>
        /// Trạng thái phê duyệt của Quiz UGC. Ví dụ: "Pending", "Approved", "Rejected".
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TrangThai { get; set; } = "Pending";

        /// <summary>
        /// Thời gian Quiz được phê duyệt/từ chối bởi Admin.
        /// </summary>
        public DateTime? NgayDuyet { get; set; }

        /// <summary>
        /// ID của Admin đã thực hiện phê duyệt (Optional).
        /// </summary>
        public int? AdminDuyetID { get; set; }

        // ===============================================
        // Navigation Properties (Điều hướng)
        // ===============================================

        [ForeignKey("UserID")]
        [JsonIgnore] // ✅ Ngăn chặn vòng lặp JSON
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        /// <summary>
        /// Danh sách các Câu Hỏi được đề xuất trong Quiz này.
        /// (Yêu cầu CauHoi.cs phải có QuizTuyChinhID là FK)
        /// </summary>
        [JsonIgnore] // ✅ Ngăn chặn vòng lặp JSON
        public virtual ICollection<CauHoi> CauHois { get; set; } = new List<CauHoi>();

        // Navigation property: 1 QuizTuyChinh có thể có nhiều QuizAttempt
        [JsonIgnore] // ✅ Ngăn chặn vòng lặp JSON (Rất quan trọng)
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }
}