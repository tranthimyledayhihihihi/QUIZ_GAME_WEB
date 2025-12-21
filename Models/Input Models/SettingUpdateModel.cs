// Models/InputModels/SettingUpdateModel.cs

using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    /// <summary>
    /// Model nhận các tùy chọn cài đặt cá nhân từ người dùng để cập nhật.
    /// </summary>
    public class SettingUpdateModel
    {
        public bool AmThanh { get; set; } = true;

        public bool NhacNen { get; set; } = true;

        public bool ThongBao { get; set; } = true;

        [Required(ErrorMessage = "Ngôn ngữ là bắt buộc.")]
        [MaxLength(20)]
        public string NgonNgu { get; set; } = "vi";
    }
}