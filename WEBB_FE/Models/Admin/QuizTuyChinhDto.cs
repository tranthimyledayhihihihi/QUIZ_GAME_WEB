using Newtonsoft.Json;
using System;

namespace WEBB.Models.Admin
{
    public class QuizTuyChinhDto
    {
        [JsonProperty("quizTuyChinhID")]
        public int QuizTuyChinhID { get; set; }

        [JsonProperty("tenQuiz")]
        public string TenQuiz { get; set; }

        [JsonProperty("moTa")]
        public string MoTa { get; set; }

        [JsonProperty("trangThai")]
        public string TrangThai { get; set; }

        [JsonProperty("ngayTao")]
        public DateTime NgayTao { get; set; }

        [JsonProperty("nguoiTao")]
        public string NguoiTao { get; set; }
    }
}
