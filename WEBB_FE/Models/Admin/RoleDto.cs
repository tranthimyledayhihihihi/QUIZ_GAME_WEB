using Newtonsoft.Json;

namespace WEBB.Models.Admin
{
    public class RoleDto
    {
        [JsonProperty("vaiTroID")]
        public int VaiTroID { get; set; }

        [JsonProperty("tenVaiTro")]
        public string TenVaiTro { get; set; }

        [JsonProperty("soLuongNguoiDung")]
        public int SoLuongNguoiDung { get; set; }
    }
}
