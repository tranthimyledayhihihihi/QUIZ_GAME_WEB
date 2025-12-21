using System;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// DTO dùng để xem chi tiết một bản ghi chia sẻ cụ thể.
    /// </summary>
    public class QuizShareDetailDto
    {
        public int QuizChiaSeID { get; set; }
        public DateTime NgayChiaSe { get; set; }

        // Thông tin người tham gia
        public int UserGuiID { get; set; }
        public string TenNguoiGui { get; set; } = null!;

        public int UserNhanID { get; set; }
        public string TenNguoiNhan { get; set; } = null!;

        // Chi tiết Quiz được chia sẻ (metadata)
        public QuizTuyChinhMetadataDto QuizMetadata { get; set; } = null!;
    }

    /// <summary>
    /// DTO chứa metadata cơ bản của Quiz Tuy Chỉnh được chia sẻ.
    /// </summary>
    public class QuizTuyChinhMetadataDto
    {
        public int QuizTuyChinhID { get; set; }
        public string TenQuiz { get; set; } = null!;
        public string? MoTa { get; set; }
        public string TrangThai { get; set; } = null!; // Trạng thái Duyệt (nếu là UGC)
        public int TongSoCauHoi { get; set; } // Số lượng câu hỏi trong Quiz đó
    }
}