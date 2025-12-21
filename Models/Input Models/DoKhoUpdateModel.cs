using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    /// <summary>
    /// Input Model dùng để cập nhật (PUT) Entity DoKho.
    /// Model này KHÔNG BAO GỒM DoKhoID.
    /// </summary>
    public class DoKhoUpdateModel
    {
        [Required(ErrorMessage = "Tên độ khó là bắt buộc.")]
        [MaxLength(50)]
        public string TenDoKho { get; set; } = null!;

        [Required(ErrorMessage = "Điểm thưởng là bắt buộc.")]
        [Range(0, 1000, ErrorMessage = "Điểm thưởng phải nằm trong khoảng từ {1} đến {2}.")]
        public int DiemThuong { get; set; } = 0;
    }
}