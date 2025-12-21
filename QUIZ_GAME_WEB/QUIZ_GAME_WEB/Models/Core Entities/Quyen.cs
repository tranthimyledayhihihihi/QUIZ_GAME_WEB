using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("Quyen")]
    public class Quyen
    {
        [Key]
        public int QuyenID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenQuyen { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        // Navigation Properties
        public virtual ICollection<VaiTro_Quyen> VaiTro_Quyens { get; set; } = new List<VaiTro_Quyen>();
    }
}
