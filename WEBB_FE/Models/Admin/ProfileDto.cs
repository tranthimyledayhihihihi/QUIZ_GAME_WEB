using Newtonsoft.Json;
using System;

namespace WEBB.Models.Admin
{
    public class ProfileDto
    {
        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("tenDangNhap")]
        public string TenDangNhap { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("hoTen")]
        public string HoTen { get; set; }

        [JsonProperty("anhDaiDien")]
        public string AnhDaiDien { get; set; }

        [JsonProperty("ngayDangKy")]
        public DateTime NgayDangKy { get; set; }

        [JsonProperty("lanDangNhapCuoi")]
        public DateTime? LanDangNhapCuoi { get; set; }

        [JsonProperty("vaiTro")]
        public string VaiTro { get; set; }
    }
}
