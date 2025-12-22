using Newtonsoft.Json;

namespace WEBB.Models.Admin
{
    public class BXHDto
    {
        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("tenDangNhap")]
        public string TenDangNhap { get; set; }

        [JsonProperty("hoTen")]
        public string HoTen { get; set; }

        [JsonProperty("tongSoTran")]
        public int TongSoTran { get; set; }

        [JsonProperty("tongSoCauDung")]
        public int TongSoCauDung { get; set; }

        [JsonProperty("diemXepHang")]
        public double DiemXepHang { get; set; }
    }
}
