using System;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// Data Transfer Object (DTO) dùng để xem CHI TIẾT một bài Quiz đã hoàn thành.
    /// </summary>
    public class KetQuaDetailDto
    {
        // Thông tin chung của kết quả bài làm
        public int QuizAttemptID { get; set; }
        public int Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCauHoi { get; set; }
        public string TrangThaiKetQua { get; set; } = null!;
        public DateTime NgayKetThuc { get; set; }

        // Chi tiết từng câu hỏi trong bài làm
        public List<CauHoiDaLamDto> ChiTietCauHoi { get; set; } = new List<CauHoiDaLamDto>();
    }

    /// <summary>
    /// DTO chứa thông tin của một câu hỏi đã được trả lời trong bài Quiz đó.
    /// </summary>
    public class CauHoiDaLamDto
    {
        // Thông tin câu hỏi
        public int CauHoiID { get; set; }
        public string NoiDung { get; set; } = null!;
        public string DapAnA { get; set; } = null!;
        public string DapAnB { get; set; } = null!;
        public string DapAnC { get; set; } = null!;
        public string DapAnD { get; set; } = null!;
        public string? HinhAnh { get; set; }

        // Kết quả của người chơi
        public string DapAnDaChon { get; set; } = null!;
        public string DapAnDung { get; set; } = null!; // ✅ RẤT QUAN TRỌNG: Hiển thị đáp án đúng để ôn tập
        public bool LaCauTraLoiDung { get; set; }

        // Thông tin metadata
        public string TenChuDe { get; set; } = null!;
        public string TenDoKho { get; set; } = null!;
    }
}