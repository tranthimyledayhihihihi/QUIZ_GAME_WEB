using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class QuizNgay
    {
        [Key]
        public int QuizNgayID { get; set; }

        public DateTime Ngay { get; set; } = DateTime.Today;

        public int? CauHoiID { get; set; }

        [ForeignKey("CauHoiID")]
        public virtual CauHoi? CauHoi { get; set; }
    }
}
