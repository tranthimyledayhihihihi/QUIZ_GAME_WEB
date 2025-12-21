// Models/Input Models/QuizCreateEditModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    public class QuizCreateEditModel
    {
        // === THÔNG TIN CẤP CAO CỦA QUIZ ===
        public int? QuizID { get; set; } // Null nếu là tạo mới

        [Required(ErrorMessage = "Tên Quiz là bắt buộc.")]
        [StringLength(100)]
        public string TenQuiz { get; set; } = null!; // Khắc phục Non-nullable

        [StringLength(255)]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Phải chọn chủ đề.")]
        public int ChuDeID { get; set; }

        [Required]
        [Range(1, 60, ErrorMessage = "Thời gian giới hạn phải từ 1 đến 60 phút.")]
        public int ThoiGianGioiHanPhut { get; set; } = 10;

        // Danh sách ID câu hỏi được chọn (Dùng khi tạo Quiz từ các câu hỏi có sẵn)
        public List<int> CauHoiIDs { get; set; } = new List<int>();

        // ====== THÔNG TIN CÂU HỎI (Dùng khi tạo/sửa Câu Hỏi đơn lẻ) ======

        // 1. CÁC THUỘC TÍNH BẮT BUỘC CHO TẠO CÂU HỎI MỚI
        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc.")]
        [MaxLength(500)]
        public string NoiDung { get; set; } = null!; // KHẮC PHỤC LỖI THIẾU

        [Required(ErrorMessage = "Phải chọn độ khó.")]
        public int DoKhoID { get; set; } // KHẮC PHỤC LỖI THIẾU

        // 2. ĐÁP ÁN (Khắc phục lỗi A, B, C, D, DapAnDung)
        [Required]
        public string A { get; set; } = null!;
        [Required]
        public string B { get; set; } = null!;
        [Required]
        public string C { get; set; } = null!;
        [Required]
        public string D { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string DapAnDung { get; set; } = null!; // Ví dụ: "A", "B", "C", "D"

        // 3. HÌNH ẢNH (Khắc phục lỗi HinhAnh)
        public string? HinhAnh { get; set; } // Dạng URL hoặc Base64
    }
}