using System.ComponentModel.DataAnnotations;

namespace WEBB.Models.User
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Để hiển thị lỗi chung trên form
        public string ErrorMessage { get; set; }
    }
}
