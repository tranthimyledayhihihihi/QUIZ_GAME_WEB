// Models/Input Models/MatchStartRequestModel.cs
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class MatchStartRequestModel
    {
        // Player1ID sẽ được lấy từ JWT Token trong Controller

        [Required(ErrorMessage = "Player2ID là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID người chơi phải hợp lệ.")]
        public int Player2ID { get; set; } // ID của người chơ  i được mời

        [Required(ErrorMessage = "Số câu hỏi là bắt buộc.")]
        [Range(5, 20, ErrorMessage = "Trận đấu phải có ít nhất 5 câu hỏi.")]
        public int SoCauHoi { get; set; } = 10;
    }
}