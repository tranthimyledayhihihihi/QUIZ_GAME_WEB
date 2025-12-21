using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    /// <summary>
    /// Input Model dùng để nhận dữ liệu từ Client khi người dùng yêu cầu đổi mật khẩu.
    /// </summary>
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Required(ErrorMessage = "Mật khẩu xác nhận là bắt buộc.")]
        public required string ConfirmNewPassword { get; set; }
    }
}