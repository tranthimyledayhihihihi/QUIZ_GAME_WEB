using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class KetQua
    {
        [Key]
        public int KetQuaID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int QuizAttemptID { get; set; }

        public int Diem { get; set; } = 0;
        public int SoCauDung { get; set; } = 0;
        public int TongCauHoi { get; set; } = 0;
        public string TrangThaiKetQua { get; set; } = "Chưa hoàn thành";
        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        [ForeignKey("QuizAttemptID")]
        public virtual QuizAttempt QuizAttempt { get; set; } = null!;
    }
}
