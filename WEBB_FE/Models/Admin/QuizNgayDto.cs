using Newtonsoft.Json;
using System;

namespace WEBB.Models.Admin
{
    public class QuizNgayDto
    {
        [JsonProperty("quizNgayID")]
        public int QuizNgayID { get; set; }

        [JsonProperty("ngay")]
        public DateTime Ngay { get; set; }

        [JsonProperty("cauHoiID")]
        public int? CauHoiID { get; set; }

        [JsonProperty("noiDungCauHoi")]
        public string NoiDungCauHoi { get; set; }
    }
}
