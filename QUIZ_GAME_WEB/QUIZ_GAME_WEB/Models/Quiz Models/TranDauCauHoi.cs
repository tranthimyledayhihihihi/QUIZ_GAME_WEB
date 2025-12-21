// Models/QuizModels/TranDauCauHoi.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json.Serialization;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class TranDauCauHoi
    {
        [Key]
        public int TranDauCauHoiID { get; set; }

        [Required]
        [ForeignKey(nameof(TranDau))]
        public int TranDauID { get; set; }

        [Required]
        [ForeignKey(nameof(CauHoi))]
        public int CauHoiID { get; set; }

        public int ThuTu { get; set; }

        // Thuộc tính điều hướng
        [JsonIgnore]
        public virtual TranDauTrucTiep TranDau { get; set; } = null!;

        [JsonIgnore]
        public virtual CauHoi CauHoi { get; set; } = null!;

    }
}