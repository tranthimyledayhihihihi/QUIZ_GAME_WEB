using System;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// Data Transfer Object (DTO) dùng để trả về thông tin hồ sơ công khai của người dùng khác.
    /// </summary>
    public class UserProfileDto
    {
        public int UserID { get; set; }

        // Thông tin công khai
        public string TenDangNhap { get; set; } = null!;
        public string? HoTen { get; set; }
        public string? AnhDaiDien { get; set; }
        public DateTime NgayDangKy { get; set; }

        // Thống kê đơn giản (Có thể tính từ Repository/Service)
        public int TongSoQuizDaLam { get; set; }
        public int TongSoCauHoiDung { get; set; }
        public int TongSoDiem { get; set; }
        public int SoNguoiTheoDoi { get; set; }
        public int DangTheoDoi { get; set; }

        // ✅ Trạng thái theo dõi (Chỉ được thiết lập khi người xem đã đăng nhập)
        public bool IsFollowing { get; set; } = false;

        // (Không bao gồm Email, MatKhau, VaiTroID, LanDangNhapCuoi)
    }
}