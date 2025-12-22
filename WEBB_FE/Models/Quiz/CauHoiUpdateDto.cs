using Newtonsoft.Json;

namespace WEBB.Models.Quiz
{
    public class CauHoiUpdateDto
    {
        [JsonProperty("cauHoiID")]
        public int CauHoiID { get; set; }

        [JsonProperty("noiDung")]
        public string NoiDung { get; set; }

        [JsonProperty("dapAnA")]
        public string DapAnA { get; set; }

        [JsonProperty("dapAnB")]
        public string DapAnB { get; set; }

        [JsonProperty("dapAnC")]
        public string DapAnC { get; set; }

        [JsonProperty("dapAnD")]
        public string DapAnD { get; set; }

        [JsonProperty("dapAnDung")]
        public string DapAnDung { get; set; }

        [JsonProperty("chuDeID")]
        public int ChuDeID { get; set; }

        [JsonProperty("doKhoID")]
        public int DoKhoID { get; set; }

        [JsonProperty("hinhAnh")]
        public string HinhAnh { get; set; }
    }
}
