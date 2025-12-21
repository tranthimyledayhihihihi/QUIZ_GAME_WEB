using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class GameStartOptions
    {
        [Required]
        public int ChuDeID { get; set; }

        [Required]
        public int DoKhoID { get; set; }

        [Required]
        [Range(5, 20)] // Giới hạn số câu hỏi từ 5 đến 20
        public int SoLuongCauHoi { get; set; } = 10;
    }
}