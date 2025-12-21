using QUIZ_GAME_WEB.Models.CoreEntities;
using System;

namespace QUIZ_GAME_WEB.Models.SocialRankingModels
{
    public class Comment
    {
        public int CommentID { get; set; }          // PK
        public int UserID { get; set; }             // FK tới NguoiDung
        public string Content { get; set; } = null!;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Nếu bạn muốn repo hoạt động với EntityType và RelatedEntityID
        public string EntityType { get; set; } = null!;
        public int RelatedEntityID { get; set; }

        // Navigation property
        public NguoiDung? User { get; set; }
    }
}
