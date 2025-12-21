using QUIZ_GAME_WEB.Models.CoreEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class QuizChiaSe
    {
        [Key]
        public int QuizChiaSeID { get; set; }

        [Required]
        public int QuizTuyChinhID { get; set; }

        [Required]
        public int UserGuiID { get; set; }

        public int? UserNhanID { get; set; }

        public DateTime NgayChiaSe { get; set; } = DateTime.Now;

        [ForeignKey("QuizTuyChinhID")]
        public virtual QuizTuyChinh QuizTuyChinh { get; set; } = null!;

        [ForeignKey("UserGuiID")]
        public virtual NguoiDung UserGui { get; set; } = null!;

        [ForeignKey("UserNhanID")]
        public virtual NguoiDung? UserNhan { get; set; }
    }
}
