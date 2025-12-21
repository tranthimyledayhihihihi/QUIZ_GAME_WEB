using System;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// Data Transfer Object (DTO) dùng để trả về thông tin chi tiết của Quiz Ngày hôm nay.
    /// </summary>
    public class QuizNgayDetailsDto
    {
        // === Thông tin Quiz Ngày ===
        public int QuizNgayID { get; set; }
        public DateTime Ngay { get; set; }

        // === Thông tin Câu Hỏi (Cần để bắt đầu chơi) ===
        public int CauHoiID { get; set; }
        public string NoiDungCauHoi { get; set; } = null!;
        public string DapAnA { get; set; } = null!;
        public string DapAnB { get; set; } = null!;
        public string DapAnC { get; set; } = null!;
        public string DapAnD { get; set; } = null!;
        public string? HinhAnh { get; set; }

        // === Thông tin Độ khó và Điểm thưởng ===
        public int DoKhoID { get; set; }
        public string TenDoKho { get; set; } = null!;
        public int DiemThuong { get; set; }

        // Lưu ý: KHÔNG bao gồm DapAnDung.
    }
}