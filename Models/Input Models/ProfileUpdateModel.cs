using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    /// <summary>
    /// Model nhận dữ liệu từ Client để cập nhật Hồ sơ người dùng.
    /// </summary>
    public class ProfileUpdateModel
    {
        // ===============================================
        // I. THÔNG TIN CƠ BẢN
        // ===============================================

        [MaxLength(100)]
        // Bỏ [Required] ở đây nếu bạn muốn cho phép người dùng chỉ cập nhật cài đặt khác.
        public string? HoTen { get; set; }

        [MaxLength(255)]
        public string? AnhDaiDien { get; set; }

        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [MaxLength(100)]
        // Bỏ [Required] để cho phép người dùng không cần cập nhật Email.
        public string? Email { get; set; }

        // ===============================================
        // II. THAY ĐỔI MẬT KHẨU (Nếu MatKhauMoi được cung cấp)
        // ===============================================

        /// <summary>
        /// Mật khẩu hiện tại (Bắt buộc phải có nếu MatKhauMoi được cung cấp).
        /// </summary>
        [DataType(DataType.Password)]
        public string? MatKhauHienTai { get; set; } // ✅ Bổ sung để xác thực

        /// <summary>
        /// Mật khẩu mới (Nếu được cung cấp, cần kiểm tra MatKhauHienTai trong Service).
        /// </summary>
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public string? MatKhauMoi { get; set; }
    }
}