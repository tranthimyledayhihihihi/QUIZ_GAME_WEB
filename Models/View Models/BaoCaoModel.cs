// Models/ViewModels/BaoCaoModel.cs (ĐÃ SỬA LỖI ĐỒNG BỘ SERVICE)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Để dùng StringLength

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    // Model dùng để tạo và xuất báo cáo (Response DTO)
    public class BaoCaoModel
    {
        // Thuộc tính cơ sở
        public string TieuDe { get; set; } = null!; // Khắc phục Non-nullable Warning
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        // ====== THUỘC TÍNH BỔ SUNG ĐỂ KHỚP VỚI REPORTINGSERVICE ======

        // Tổng hợp KPI
        public int TongSoLanChoi { get; set; }
        public float TyLeTraLoiDungTrungBinh { get; set; }

        // Chi tiết thống kê theo ngày (Khắc phục lỗi ChiTietTheoNgay)
        public List<ChiTietNgayModel> ChiTietTheoNgay { get; set; } = new List<ChiTietNgayModel>();

        // Dữ liệu thô (Nếu bạn muốn giữ lại cấu trúc ban đầu của bạn)
        public List<string> CotDuLieu { get; set; } = new List<string>();
        public List<List<string>> DongDuLieu { get; set; } = new List<List<string>>();
    }

    // Lưu ý: Class ChiTietNgayModel cũng cần được định nghĩa trong ViewModels
}