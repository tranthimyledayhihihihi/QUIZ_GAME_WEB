using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class CaiDatNguoiDung
    {
        [Key]
        public int SettingID { get; set; }
        public int UserID { get; set; }
        public bool AmThanh { get; set; } = true;
        public bool NhacNen { get; set; } = true;
        public bool ThongBao { get; set; } = true;
        public string NgonNgu { get; set; } = "vi";
    }
}
