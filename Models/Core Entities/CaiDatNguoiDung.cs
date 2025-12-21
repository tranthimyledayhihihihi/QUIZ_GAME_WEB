using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("CaiDatNguoiDung")]
    public class CaiDatNguoiDung
    {
        [Key]
        public int SettingID { get; set; }

        [Required]
        [ForeignKey("NguoiDung")]
        public int UserID { get; set; }

        public bool AmThanh { get; set; } = true;
        public bool NhacNen { get; set; } = true;
        public bool ThongBao { get; set; } = true;

        [MaxLength(20)]
        public string NgonNgu { get; set; } = "vi";

        // Navigation
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
