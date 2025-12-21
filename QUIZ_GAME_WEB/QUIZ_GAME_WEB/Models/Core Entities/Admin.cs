using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [Required]
        [ForeignKey("NguoiDung")]
        public int UserID { get; set; }

        [Required]
        [ForeignKey("VaiTro")]
        public int VaiTroID { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Navigation Properties
        public virtual NguoiDung User { get; set; } = null!;
        public virtual VaiTro VaiTro { get; set; } = null!;
    }
}
