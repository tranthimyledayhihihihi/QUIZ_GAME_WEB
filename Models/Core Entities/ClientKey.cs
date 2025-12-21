// Models/Core Entities/ClientKey.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.CoreEntities // Namespace đã được sửa
{
    public class ClientKey
    {
        [Key]
        public int ClientKeyID { get; set; } // Khóa chính tự động tăng

        [Required]
        [MaxLength(255)]
        public string KeyValue { get; set; } // Giá trị API Key thực tế (Key bí mật)

        [Required]
        [MaxLength(100)]
        public string TenUngDung { get; set; } // Ví dụ: "WebClient", "MobileApp"

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayHetHan { get; set; } // Cho phép key hết hạn

        public bool IsActive { get; set; } = true; // 1: Key hoạt động, 0: Đã bị vô hiệu hóa
    }
}