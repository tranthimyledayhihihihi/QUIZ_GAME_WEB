using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Cần thiết

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        public int VaiTroID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenVaiTro { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        // Navigation Properties

        // 1. Quan hệ 1:N với Admin
        [JsonIgnore] // Ngăn JSON cycle nếu trả về Admin
        public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

        // 2. Quan hệ N:N với VaiTro_Quyen (Giữ nguyên)
        public ICollection<VaiTro_Quyen> VaiTro_Quyens { get; set; } = new List<VaiTro_Quyen>();

        // 3. ✅ KHẮC PHỤC LỖI: Bổ sung Navigation Property 1:N đến NguoiDung
        /// <summary>
        /// Danh sách người dùng có vai trò này.
        /// </summary>
        [JsonIgnore] // Ngăn JSON cycle khi trả về NguoiDung
        public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
    }
}