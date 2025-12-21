using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Cần thiết

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("PhienDangNhap")]
    public class PhienDangNhap
    {
        [Key]
        public int SessionID { get; set; }

        [Required]
        [ForeignKey(nameof(NguoiDung))]
        public int UserID { get; set; }

        [MaxLength(500)]
        public string? Token { get; set; }

        // ✅ THAY ĐỔI: Phù hợp với Controller QLPhienDangNhapController
        [Required]
        public DateTime ThoiGianBatDau { get; set; } = DateTime.UtcNow; // Sửa từ ThoiGianDangNhap

        /// <summary>
        /// Null nếu phiên đang hoạt động.
        /// </summary>
        public DateTime? ThoiGianKetThuc { get; set; } // Sửa từ ThoiGianHetHan

        // ✅ BỔ SUNG: Khắc phục lỗi biên dịch
        [MaxLength(45)]
        public string? IPAddress { get; set; }

        [MaxLength(50)]
        public string? DeviceType { get; set; }

        // Cột TrangThai (true/false) có thể dư thừa nếu dùng ThoiGianKetThuc, 
        // nhưng giữ lại nếu logic cũ sử dụng.
        public bool TrangThai { get; set; } = true;

        // Navigation
        // [JsonIgnore] có thể cần ở đây nếu NguoiDung cũng có Collection PhienDangNhaps
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}