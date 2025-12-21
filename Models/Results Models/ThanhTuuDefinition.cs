using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    [Table("ThanhTuuDefinition")]
    public class ThanhTuuDefinition // Entity dùng cho Admin Controller (QLThanhTuuController)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DefinitionID { get; set; }

        [Required]
        [MaxLength(50)]
        public string AchievementCode { get; set; } = null!; // Mã code (Ví dụ: MASTER_100_QUIZ)

        [Required]
        [MaxLength(100)]
        public string TenThanhTuu { get; set; } = null!;

        [MaxLength(500)]
        public string? MoTa { get; set; }

        [MaxLength(255)]
        public string? BieuTuong { get; set; }

        // Cột mới: Loại điều kiện (Giúp code logic)
        [Required]
        [MaxLength(50)]
        public string LoaiDieuKien { get; set; } = null!; // Ví dụ: 'QuizCount', 'Streak', 'Score'

        [Required]
        public int GiaTriCanThiet { get; set; } // Ngưỡng đạt được (ví dụ: 100)

        [Required]
        public int DiemThuong { get; set; } = 0; // Phần thưởng

        // === ADMIN TRACKING ===

        [Required]
        [ForeignKey("Admin")]
        public int AdminID { get; set; }

        // Navigation Properties
        public virtual Admin Admin { get; set; } = null!;

        // Danh sách các bản ghi ThanhTuu đã đạt được trỏ đến định nghĩa này
        public virtual ICollection<ThanhTuu> ThanhTuuDaDat { get; set; } = new HashSet<ThanhTuu>();
    }
}