using Newtonsoft.Json;
using System;

namespace WEBB.Models.Quiz
{
    public class CauHoiDto
    {
        [JsonProperty("cauHoiID")]
        public int CauHoiID { get; set; }

        [JsonProperty("chuDeID")]
        public int ChuDeID { get; set; }

        [JsonProperty("doKhoID")]
        public int DoKhoID { get; set; }

        [JsonProperty("quizTuyChinhID")]
        public int QuizTuyChinhID { get; set; } = 0;

        [JsonProperty("adminDuyetID")]
        public int AdminDuyetID { get; set; } = 0;

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

        [JsonProperty("hinhAnh")]
        public string HinhAnh { get; set; }

        [JsonProperty("ngayTao")]
        public DateTime? NgayTao { get; set; }

        [JsonProperty("trangThaiDuyet")]
        public string TrangThaiDuyet { get; set; }

        [JsonProperty("cacLuaChon")]
        public string CacLuaChon { get; set; }

        // ====== chỉ dùng hiển thị ======
        public string TenChuDe { get; set; }
        public string TenDoKho { get; set; }
    }
}
