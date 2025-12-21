using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class ThanhTuu
    {
        [Key]
        public int ThanhTuuID { get; set; }

        [MaxLength(100)]
        public string? TenThanhTuu { get; set; }

        public string? MoTa { get; set; }

        public string? BieuTuong { get; set; }

        public string? DieuKien { get; set; }

        [Required]
        public int NguoiDungID { get; set; }

        public string AchievementCode { get; set; } = null!;

        [ForeignKey("NguoiDungID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
