using Newtonsoft.Json;

namespace WEBB.Models.Admin
{
    public class TroGiupDto
    {
        [JsonProperty("troGiupID")]
        public int TroGiupID { get; set; }

        [JsonProperty("tenTroGiup")]
        public string TenTroGiup { get; set; }

        [JsonProperty("moTa")]
        public string MoTa { get; set; }
    }
}
