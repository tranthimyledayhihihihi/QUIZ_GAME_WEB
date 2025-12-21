// Models/ViewModels/AdminDashboardModel.cs
using System.Collections.Generic;
using System;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    // Cần phải có định nghĩa LogItemModel trong cùng Namespace/File

    // Model cho trang Dashboard của Admin
    public class AdminDashboardModel
    {
        public int TongSoNguoiDung { get; set; }
        public int NguoiDungMoiHomNay { get; set; }
        public int TongSoCauHoi { get; set; } // Thay thế cho TongSoQuizDaTao
        public int TongSoChuDe { get; set; }
        public int SoTranDauHomNay { get; set; }

        // ====== THUỘC TÍNH BỔ SUNG ĐỂ KHẮC PHỤC LỖI SERVICE ======

        // Cần cho IAdminDashboardService và AdminDashboardService.cs
        public int TongSoQuizDaTao { get; set; } // Phải có để khớp với Service cũ
        public int TongSoLuotChoiHomNay { get; set; }
        public float TyLeCauTraLoiDungTrungBinh { get; set; }

        // Dùng cho biểu đồ (biến thể của ThongKeViewModel)
        public Dictionary<string, int> ThongKeHoatDongThang { get; set; } = new Dictionary<string, int>();

        // Dùng cho Log/Hoạt động gần đây (Khắc phục lỗi LogItemModel)
        public List<LogItemModel> LogHoatDongGanNhat { get; set; } = new List<LogItemModel>();
    }

    // BỔ SUNG: Định nghĩa Model phụ trợ LogItemModel (đặt ở đây nếu không tạo file riêng)
    public class LogItemModel
    {
        public int LogID { get; set; }
        public DateTime ThoiGian { get; set; }
        public string NguoiThucHien { get; set; } = string.Empty;
        public string HoatDong { get; set; } = string.Empty;
    }
}