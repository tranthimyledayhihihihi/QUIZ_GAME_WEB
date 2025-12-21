using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.SocialRankingModels
{
    public class NguoiDungOnline
    {
        [Key]
        public int OnlineID { get; set; }
        [Required]
        public int UserID { get; set; }
        [MaxLength(20)]
        public string TrangThai { get; set; } = "Online"; // "Online", "Offline", "Playing"
        public DateTime ThoiGianCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}