using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.ComponentModel.DataAnnotations.Schema; // Cần thiết cho các Entity

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class ChuDe
    {
        [Key]
        public int ChuDeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenChuDe { get; set; } = null!;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        /// <summary>
        /// Collection của tất cả các câu hỏi thuộc chủ đề này.
        /// [JsonIgnore] ngăn vòng lặp serialization: ChuDe -> CauHoi -> ChuDe
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CauHoi> CauHois { get; set; } = new HashSet<CauHoi>();
    }
}