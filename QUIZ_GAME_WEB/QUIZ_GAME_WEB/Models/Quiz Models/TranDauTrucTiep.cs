// Models/QuizModels/TranDauTrucTiep.cs
using QUIZ_GAME_WEB.Models.CoreEntities; // Cần cho NguoiDung
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.ResultsModels; // Cần để tìm thấy TraLoiTrucTiep

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class TranDauTrucTiep
    {
        [Key]
        public int TranDauID { get; set; }

        [Required]
        [ForeignKey(nameof(Player1))]
        public int Player1ID { get; set; } // Người tạo/mời

        [Required]
        [ForeignKey(nameof(Player2))]
        public int Player2ID { get; set; } // Người được mời

        public int SoCauHoi { get; set; }
        public int DiemPlayer1 { get; set; } = 0;
        public int DiemPlayer2 { get; set; } = 0;

        public int? WinnerUserID { get; set; } // Cho phép NULL nếu hòa/chưa xong

        [Required]
        [MaxLength(20)]
        public string TrangThai { get; set; } = "DangCho"; // 'DangCho','DangChoi','HoanThanh','Huy'
        [Required]
        [MaxLength(10)]
        public string MatchCode { get; set; } = null!;

        public DateTime ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }

        // Thuộc tính điều hướng
        public virtual NguoiDung Player1 { get; set; } = null!;
        public virtual NguoiDung Player2 { get; set; } = null!;
        public virtual NguoiDung? Winner { get; set; } // Mối quan hệ tùy chọn

        // Mối quan hệ N:M với Câu Hỏi
        public virtual ICollection<TranDauCauHoi> TranDauCauHois { get; set; } = new HashSet<TranDauCauHoi>();

        // Mối quan hệ 1:N với Log trả lời
        public virtual ICollection<TraLoiTrucTiep> TraLoiTrucTieps { get; set; } = new HashSet<TraLoiTrucTiep>();
    }
}