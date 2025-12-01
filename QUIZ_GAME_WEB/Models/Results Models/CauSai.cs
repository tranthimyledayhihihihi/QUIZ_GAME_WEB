using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class CauSai
    {
        [Key]
        public int CauSaiID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int CauHoiID { get; set; }

        [Required]
        public int QuizAttemptID { get; set; }

        public DateTime NgaySai { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        [ForeignKey("CauHoiID")]
        public virtual CauHoi CauHoi { get; set; } = null!;

        [ForeignKey("QuizAttemptID")]
        public virtual QuizAttempt QuizAttempt { get; set; } = null!;
    }
}
