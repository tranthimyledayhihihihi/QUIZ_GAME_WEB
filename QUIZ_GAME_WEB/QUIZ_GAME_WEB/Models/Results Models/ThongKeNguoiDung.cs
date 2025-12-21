using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class ThongKeNguoiDung
    {
        [Key]
        public int ThongKeID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime Ngay { get; set; } = DateTime.Today;

        public int SoTran { get; set; } = 0;

        public int SoCauDung { get; set; } = 0;

        public double DiemTrungBinh { get; set; } = 0;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
