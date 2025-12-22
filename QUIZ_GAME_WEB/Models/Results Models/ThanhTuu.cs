using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;
using System; // Cần thiết cho DateTime.Now nếu bạn thêm thời gian đạt được

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    [Table("ThanhTuu")]
    public class ThanhTuu // Thành tựu đã đạt được (User-Earned)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ThanhTuuID { get; set; }

        // ✅ THAY THẾ: Trỏ đến Định nghĩa chung (ThanhTuuDefinition)
        [Required]
        [ForeignKey("ThanhTuuDefinition")]
        public int DefinitionID { get; set; }

        // --- CÁC THUỘC TÍNH KHÁC GIỮ NGUYÊN ---

        [Required]
        [ForeignKey("NguoiDung")]
        public int NguoiDungID { get; set; }

        // Vẫn giữ lại AchievementCode nếu cần thiết cho logic truy vấn nhanh
        public string AchievementCode { get; set; } = null!;

        // 💡 Nên thêm thời gian đạt được
        public DateTime NgayDatDuoc { get; set; } = DateTime.Now;

        // ❌ ĐÃ LOẠI BỎ CÁC THUỘC TÍNH TRÙNG LẶP (TenThanhTuu, MoTa, BieuTuong, DieuKien)

        // Navigation Properties
        public virtual NguoiDung NguoiDung { get; set; } = null!;
        public virtual ThanhTuuDefinition Definition { get; set; } = null!; // ✅ Navigation Property mới
    }
}