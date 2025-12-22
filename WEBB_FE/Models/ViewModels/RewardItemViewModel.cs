using System;

namespace WEBB.Models.ViewModels
{
    public class RewardItemViewModel
    {
        public int ThuongID { get; set; }
        public int UserID { get; set; }
        public DateTime NgayNhan { get; set; }
        public string PhanThuong { get; set; }
        public int DiemThuong { get; set; }
        public bool TrangThaiNhan { get; set; }

        public RewardItemViewModel()
        {
            PhanThuong = string.Empty;
        }
    }
}
