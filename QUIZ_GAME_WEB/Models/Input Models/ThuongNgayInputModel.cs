using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class ThuongNgayInputModel
    {
        [Required(ErrorMessage = "Phải chọn người dùng.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Tên phần thưởng không được để trống.")]
        [MaxLength(100)]
        public string PhanThuong { get; set; } = null!;

        [Range(0, 10000, ErrorMessage = "Điểm thưởng phải từ 0 đến 10,000.")]
        public int DiemThuong { get; set; }

        public bool TrangThaiNhan { get; set; } = true;
    }
}