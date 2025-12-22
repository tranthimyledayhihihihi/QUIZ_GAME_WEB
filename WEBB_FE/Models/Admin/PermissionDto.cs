using Newtonsoft.Json;

namespace WEBB.Models.Admin
{
    public class PermissionDto
    {
        [JsonProperty("quyenID")]
        public int QuyenID { get; set; }

        [JsonProperty("tenQuyen")]
        public string TenQuyen { get; set; }

        [JsonProperty("moTa")]
        public string MoTa { get; set; }
    }
}
