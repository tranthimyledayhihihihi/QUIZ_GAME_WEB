using System;
using System.Collections.Generic;

namespace WEBB.Models.ViewModels
{
    public class HistoryListViewModel
    {
        public int TongSoKetQua { get; set; }
        public int TrangHienTai { get; set; }
        public int KichThuocTrang { get; set; }
        public int TongSoTrang { get; set; }
        public List<HistoryItemDto> DanhSach { get; set; }

        public HistoryListViewModel()
        {
            DanhSach = new List<HistoryItemDto>();
        }
    }

    public class HistoryItemDto
    {
        public int QuizAttemptID { get; set; }
        public int UserID { get; set; }
        public string TenQuiz { get; set; } // Nếu API có trả về
        public double Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongSoCau { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        
        // Property tiện ích để hiển thị view
        public DateTime NgayChoi => NgayBatDau;
        public string KetQua => SoCauDung >= (TongSoCau / 2) ? "Đạt" : "Chưa đạt"; // Logic giả định
    }
}
