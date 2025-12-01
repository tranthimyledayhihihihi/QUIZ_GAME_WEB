using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class QuizAttempt
    {
        [Key]
        public int QuizAttemptID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int QuizTuyChinhID { get; set; }

        public DateTime NgayBatDau { get; set; } = DateTime.Now;
        public DateTime? NgayKetThuc { get; set; }

        public int SoCauHoiLam { get; set; } = 0;
        public int SoCauDung { get; set; } = 0;
        public int Diem { get; set; } = 0;

        // Thêm trạng thái: "DangLam", "HoanThanh", "BoDo"
        [Required]
        public string TrangThai { get; set; } = "DangLam";

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        [ForeignKey("QuizTuyChinhID")]
        public virtual QuizTuyChinh QuizTuyChinh { get; set; } = null!;

        // Navigation: danh sách các câu sai trong attempt này
        public virtual ICollection<CauSai> CauSais { get; set; } = new List<CauSai>();

        // Navigation: kết quả cuối cùng (optional)
        public virtual KetQua? KetQua { get; set; }
    }
}
