// Models/Input Models/BaoCaoRequestModel.cs (ĐÃ SỬA LỖI ĐỒNG BỘ)
using System;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels
{
    // Model để nhận yêu cầu tạo báo cáo
    public class BaoCaoRequestModel
    {
        // 1. KHẮC PHỤC LỖI THIẾU PROPERTY: UserID (Bắt buộc cho logic Service)
        [Required(ErrorMessage = "UserID là bắt buộc cho báo cáo.")]
        public int UserID { get; set; }

        // 2. Thuộc tính cơ bản
        [Required]
        public string LoaiBaoCao { get; set; } = null!; // Sửa cảnh báo Non-nullable

        public DateTime? NgayBatDau { get; set; } // Ngày bắt đầu lọc (Có thể null)
        public DateTime? NgayKetThuc { get; set; } // Ngày kết thúc lọc (Có thể null)

        public int? ChuDeID { get; set; } // Lọc theo chủ đề (bổ sung tùy chọn)
    }
}