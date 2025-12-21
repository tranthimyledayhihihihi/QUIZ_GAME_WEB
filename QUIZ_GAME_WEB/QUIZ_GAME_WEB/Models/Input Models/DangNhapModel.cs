using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class DangNhapModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public required string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public required string MatKhau { get; set; }
    }
}