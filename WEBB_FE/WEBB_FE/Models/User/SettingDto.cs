using System.ComponentModel.DataAnnotations;

namespace WEBB.Models.User
{
    public class SettingDto
    {
        public bool AmThanh { get; set; } = true;
        public bool NhacNen { get; set; } = true;
        public bool ThongBao { get; set; } = true;

        [Required]
        [MaxLength(20)]
        public string NgonNgu { get; set; } = "vi";
    }
}
