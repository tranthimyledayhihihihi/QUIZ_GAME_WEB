using System;
using System.Collections.Generic;

namespace WEBB.Models.ViewModels
{
    public class DailyStreakViewModel
    {
        // Core streak
        public int SoNgayLienTiep { get; set; }
        public DateTime? NgayCapNhatCuoi { get; set; }

        // UI info
        public string Message { get; set; }
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }

        // Timeline 7 ngày
        public List<DateTime> LichSu7Ngay { get; set; }

        public DailyStreakViewModel()
        {
            LichSu7Ngay = new List<DateTime>();
        }
    }
}