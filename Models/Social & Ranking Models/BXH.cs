// Models/SocialRankingModels/BXH.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.SocialRankingModels
{
    [Table("BXH")]
    public class BXH
    {
        [Key]
        public int BXHID { get; set; }

        // FK tới NguoiDung
        [Required]
        public int UserID { get; set; }

        // Điểm tuần/tháng
        public int DiemTuan { get; set; }
        public int DiemThang { get; set; }

        // Hạng tuần/tháng
        public int HangTuan { get; set; }
        public int HangThang { get; set; }

        // Navigation property tới NguoiDung
        [ForeignKey("UserID")]
        public NguoiDung? NguoiDung { get; set; }
    }
}
