using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class ChuoiNgay
    {
        [Key]
        public int ChuoiID { get; set; }

        [Required]
        public int UserID { get; set; }

        public int SoNgayLienTiep { get; set; } = 0;

        public DateTime NgayCapNhatCuoi { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
