// Models/Core Entities/SystemSetting.cs
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.CoreEntities // Namespace đã được sửa
{
    public class SystemSetting
    {
        // Key là Primary Key (Khóa Chính) cho mô hình Key-Value
        [Key]
        [MaxLength(100)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; } // Giá trị cấu hình (ví dụ: "10", "on", "vi")

        [MaxLength(255)]
        public string? MoTa { get; set; } // Mô tả để Admin dễ hiểu
    }
}