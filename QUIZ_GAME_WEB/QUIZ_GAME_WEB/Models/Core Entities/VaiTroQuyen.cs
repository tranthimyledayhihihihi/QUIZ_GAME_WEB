using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("VaiTro_Quyen")]
    public class VaiTro_Quyen
    {
        public int VaiTroID { get; set; }
        public int QuyenID { get; set; }

        // Navigation
        public virtual VaiTro VaiTro { get; set; } = null!;
        public virtual Quyen Quyen { get; set; } = null!;
    }
}
