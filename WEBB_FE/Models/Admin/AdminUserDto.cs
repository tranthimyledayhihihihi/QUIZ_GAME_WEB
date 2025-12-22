using System;

namespace WEBB.Models.Admin
{
    public class AdminUserDto
    {
        public int UserID { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; } // Mặc dù mật khẩu thường không được hiển thị, nhưng có thể cần nếu bạn làm việc với bảo mật
        public string Email { get; set; }
        public string HoTen { get; set; }
        public string AnhDaiDien { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; }
        public int VaiTroID { get; set; }
        public string RoleName { get; set; }  // Tên vai trò như "Admin", "Player", "Moderator"
    }
}
