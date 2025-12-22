namespace QUIZ_GAME_WEB.Models.InputModels
{
    // Models/InputModels/DoKhoCreateModel.cs

    using System.ComponentModel.DataAnnotations;

    namespace QUIZ_GAME_WEB.Models.InputModels
    {
        public class DoKhoCreateModel
        {
            [Required(ErrorMessage = "Tên độ khó là bắt buộc.")]
            [MaxLength(50)]
            public string TenDoKho { get; set; } = null!;

            [Required(ErrorMessage = "Điểm thưởng là bắt buộc.")]
            [Range(0, 1000)]
            public int DiemThuong { get; set; } = 0;

            // KHÔNG BAO GỒM DoKhoID
        }
    }
}
