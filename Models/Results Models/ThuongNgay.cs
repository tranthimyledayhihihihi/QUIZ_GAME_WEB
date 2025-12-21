using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class ThuongNgay
    {
        [Key]
        public int ThuongID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime NgayNhan { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(100)]
        public string PhanThuong { get; set; } = null!;

        public int DiemThuong { get; set; } = 0;

        public bool TrangThaiNhan { get; set; } = false;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
