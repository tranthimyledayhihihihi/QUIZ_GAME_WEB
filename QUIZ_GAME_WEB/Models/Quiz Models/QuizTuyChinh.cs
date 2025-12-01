using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ResultsModels;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class QuizTuyChinh
    {
        [Key]
        public int QuizTuyChinhID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenQuiz { get; set; } = null!;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        // Navigation property: 1 QuizTuyChinh có thể có nhiều QuizAttempt
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }
}
