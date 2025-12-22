using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WEBB_FE.Models.Admin
{
    // =========================
    // DAILY STATS DTO
    // =========================
    public class ThongKeDailyDto
    {
        [JsonProperty("thongKeID")]
        public int ThongKeID { get; set; }

        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("ngay")]
        public DateTime Ngay { get; set; }

        [JsonProperty("soTran")]
        public int SoTran { get; set; }

        [JsonProperty("soCauDung")]
        public int SoCauDung { get; set; }

        [JsonProperty("diemTrungBinh")]
        public double DiemTrungBinh { get; set; }
    }

    // =========================
    // STREAK DTO
    // =========================
    public class ThongKeStreakDto
    {
        [JsonProperty("chuoiID")]
        public int ChuoiID { get; set; }

        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("soNgayLienTiep")]
        public int SoNgayLienTiep { get; set; }

        [JsonProperty("ngayCapNhatCuoi")]
        public DateTime NgayCapNhatCuoi { get; set; }
    }

    // =========================
    // VIEW MODEL
    // =========================
    public class ThongKeViewModel
    {
        public int UserID { get; set; }
        public List<ThongKeDailyDto> DailyStats { get; set; } = new List<ThongKeDailyDto>();
        public ThongKeStreakDto Streak { get; set; }
    }
}
