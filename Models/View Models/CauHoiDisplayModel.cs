using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// ViewModel được sử dụng để hiển thị câu hỏi cho người chơi.
    /// KHÔNG BAO GỒM ĐÁP ÁN ĐÚNG.
    /// </summary>
    public class CauHoiDisplayModel
    {
        [Required]
        public int CauHoiID { get; set; } // ID duy nhất để Client gửi đáp án

        [Required]
        public string NoiDung { get; set; } = string.Empty; // Nội dung câu hỏi

        [Required]
        public string CacLuaChon { get; set; } = string.Empty; // Chuỗi JSON/XML hoặc định dạng văn bản chứa các lựa chọn (A, B, C, D)

        // Các trường tùy chọn để làm giàu trải nghiệm/xử lý

        /// <summary>
        /// Thứ tự câu hỏi trong trận đấu hiện tại (từ 1 đến 10).
        /// </summary>
        public int ThuTuTrongTranDau { get; set; }

        /// <summary>
        /// (Tùy chọn) Thời gian tối đa để trả lời câu hỏi này (ví dụ: 15 giây).
        /// </summary>
        public double ThoiGianToiDa { get; set; } = 15.0;

        /// <summary>
        /// (Tùy chọn) Mã chủ đề (Topic ID) của câu hỏi.
        /// </summary>
        public int? ChuDeID { get; set; }
    }
}