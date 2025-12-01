namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// Data Transfer Object (DTO) dùng để trả về thông tin chi tiết của Câu Hỏi
    /// khi duyệt, tìm kiếm hoặc hiển thị thống kê.
    /// </summary>
    public class CauHoiInfoDto
    {
        // === Thông tin cốt lõi của câu hỏi (Giống như CauHoiPlayDto) ===
        public int CauHoiID { get; set; }
        public string NoiDung { get; set; } = null!;
        public string DapAnA { get; set; } = null!;
        public string DapAnB { get; set; } = null!;
        public string DapAnC { get; set; } = null!;
        public string DapAnD { get; set; } = null!;
        public string? HinhAnh { get; set; }

        // === Thông tin bổ sung từ Navigation Properties (ChuDe và DoKho) ===

        public int ChuDeID { get; set; }
        public string TenChuDe { get; set; } = null!; // Tên Chủ đề

        public int DoKhoID { get; set; }
        public string TenDoKho { get; set; } = null!; // Tên Độ khó
        public int DiemThuong { get; set; } // Điểm thưởng cho độ khó này

        // KHÔNG BAO GỒM DapAnDung
    }
}