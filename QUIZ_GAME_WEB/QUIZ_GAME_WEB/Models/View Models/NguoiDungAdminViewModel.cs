using System; // Cần cho DateTime và DateTime?

namespace QUIZ_GAME_WEB.Models
{
    /// <summary>
    /// ViewModel để hiển thị thông tin chi tiết về người dùng trong Bảng điều khiển Admin.
    /// </summary>
    public class NguoiDungAdminViewModel
    {
        public int UserID { get; set; }

        // Thông tin cơ bản
        public string? TenDangNhap { get; set; }
        public string? Email { get; set; }
        public string? HoTen { get; set; }

        // Thông tin về Vai trò (Cần thêm sau này)
        public string? VaiTro { get; set; } // Ví dụ: "Admin", "Player", "SuperAdmin"

        // Thông tin thời gian
        public DateTime NgayDangKy { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }

        // Trạng thái (Quan trọng cho Lockout)
        public bool TrangThai { get; set; } // True = Hoạt động, False = Bị khóa/Ngừng hoạt động
    }
}