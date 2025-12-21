// WEBB/Models/ViewModels/DailyRewardViewModel.cs
using System;

namespace WEBB.Models.ViewModels
{
    public class DailyRewardViewModel
    {
        public int SoNgayLienTiep { get; set; }
        public DateTime? NgayCapNhatCuoi { get; set; }

        // Thông báo lần load đầu
        public string Message { get; set; }
    }
}
