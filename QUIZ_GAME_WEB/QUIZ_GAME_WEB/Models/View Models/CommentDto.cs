// Models/ViewModels/CommentDto.cs
using System;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    public class CommentDto
    {
        public int CommentID { get; set; }
        public string Content { get; set; } = null!;
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; } // Nếu bạn thêm cột này

        // Thông tin người dùng
        public int UserID { get; set; }
        public string TenNguoiDung { get; set; } = null!;
        public string? AnhDaiDien { get; set; }

        // Thông tin tài nguyên
        public string EntityType { get; set; } = null!;
        public int RelatedEntityID { get; set; }
    }
}