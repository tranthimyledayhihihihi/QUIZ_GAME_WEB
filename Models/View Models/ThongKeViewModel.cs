// Models/ViewModels/ThongKeViewModel.cs
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    // Model dùng cho trang thống kê cá nhân
    public class ThongKeViewModel
    {
        // Các thuộc tính thống kê cơ bản
        public int TongSoTranDaChoi { get; set; }
        public int TongSoCauDung { get; set; }
        public double TyLeThang { get; set; }
        public int DiemCaoNhat { get; set; }
        public int ChuoiNgayDaiNhat { get; set; }

        // ====== THUỘC TÍNH BỔ SUNG CHO BIỂU ĐỒ (Dùng trong ReportingService) ======

        public string TenThongKe { get; set; } = "Thống kê Hiệu suất";
        public string LoaiBieuDo { get; set; } = "LineChart";

        // Các nhãn trên trục X (ví dụ: Tuan 1, Tuan 2...)
        public List<string> Labels { get; set; } = new List<string>();

        // Dữ liệu cho các đường biểu đồ (ví dụ: Tỉ lệ đúng, Số câu trả lời)
        public List<ThongKeSeries> DataSeries { get; set; } = new List<ThongKeSeries>();
    }

    // Class phụ trợ để định nghĩa dữ liệu cho một đường biểu đồ
    public class ThongKeSeries
    {
        public string TenSeries { get; set; } = string.Empty;
        public List<float> Values { get; set; } = new List<float>();
    }
}