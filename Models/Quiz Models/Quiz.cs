using QUIZ_GAME_WEB.Models.CoreEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class Quiz
    {
        [Key]
        public int QuizID { get; set; }

        [Required]
        [StringLength(150)]
        public string TenQuiz { get; set; } = null!;

        [StringLength(500)]
        public string? MoTa { get; set; }

        public int SoLuongCauHoi { get; set; }

        public int ThoiGianGioiHanPhut { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Admin")]
        public int AdminID { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual Admin Admin { get; set; } = null!;
    }
}
