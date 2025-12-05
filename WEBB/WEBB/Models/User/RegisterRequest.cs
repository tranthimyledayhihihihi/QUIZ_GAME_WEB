namespace WEBB.Models.User
{
    public class RegisterRequest
    {
        public string UserName { get; set; }      // Tên đăng nhập
        public string Email { get; set; }         // Email
        public string Password { get; set; }      // Mật khẩu
        public string ConfirmPassword { get; set; } // Xác nhận mật khẩu

        public string ErrorMessage { get; set; }  // Hiển thị lỗi từ API
    }
}
