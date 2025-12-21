using System;
using System.Collections.Generic;

namespace WEBB.Models.ViewModels
{
    public class DailyStreakViewModel
    {
        // Core streak
        public int SoNgayLienTiep { get; set; }
        public DateTime? NgayCapNhatCuoi { get; set; }

        // Reward state
        public bool DaNhanThuongHomNay { get; set; }
        public bool CoTheNhanThuong { get; set; }

        // Reward info
        public int DiemThuong { get; set; }
        public int BonusMultiplier { get; set; }
        public int TongDiemDaNhan { get; set; }

        // History
        public List<DateTime> LichSu7Ngay { get; set; }

        // UI helpers
        public string Message { get; set; }
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }

        public DailyStreakViewModel()
        {
            LichSu7Ngay = new List<DateTime>();
            SoNgayLienTiep = 0;
            DiemThuong = 10;
            BonusMultiplier = 1;
            TongDiemDaNhan = 0;
            CoTheNhanThuong = true;
            DaNhanThuongHomNay = false;
            Message = "Hãy bắt đầu chuỗi ngày của bạn!";
        }
    }
}
