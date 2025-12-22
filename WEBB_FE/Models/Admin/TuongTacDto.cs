using Newtonsoft.Json;
using System;

namespace WEBB.Models.Admin
{
    public class TuongTacDto
    {
        [JsonProperty("commentID")]
        public int CommentID { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("ngayTao")]
        public DateTime NgayTao { get; set; }

        [JsonProperty("entityType")]
        public string EntityType { get; set; }

        [JsonProperty("relatedEntityID")]
        public int RelatedEntityID { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
