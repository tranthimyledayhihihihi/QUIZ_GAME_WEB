using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    public class MatchResultModel
    {
        [Required]
        public string MatchCode { get; set; } = null!;  // 🔥 Mã trận đấu duy nhất

        [Required]
        public string KetQua { get; set; } = "HoanThanh"; // Thắng / Thua / Hòa

        [Required]
        public int DiemPlayer1 { get; set; }

        [Required]
        public int DiemPlayer2 { get; set; }

        public int Player1ID { get; set; } // <--- Thêm mới
        public int Player2ID { get; set; } // <--- Thêm mới

        public string? WinnerHoTen { get; set; } // Tên người thắng hoặc "Hòa"

        // Tuỳ chọn: tổng thưởng sau trận
        public int DiemThuongNhanDuoc { get; set; } = 0;
    }
}
