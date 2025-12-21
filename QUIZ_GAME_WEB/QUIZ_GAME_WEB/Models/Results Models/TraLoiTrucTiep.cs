// Models/ResultsModels/TraLoiTrucTiep.cs (Đặt trong ResultsModels vì nó là log kết quả)
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class TraLoiTrucTiep
    {
        [Key]
        public int TraLoiID { get; set; }

        [Required]
        [ForeignKey(nameof(TranDau))]
        public int TranDauID { get; set; }

        [Required]
        [ForeignKey(nameof(CauHoi))]
        public int CauHoiID { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }

        [Required]
        [MaxLength(10)]
        public string DapAnNguoiChoi { get; set; } = null!;

        public bool DungHaySai { get; set; }

        public DateTime ThoiGianTraLoi { get; set; }

        public int DiemNhanDuoc { get; set; } = 0;
        // Trong Models/ResultsModels/TraLoiTrucTiep.cs
        // (Bổ sung)

        public double ThoiGianGiaiQuyet { get; set; } // Thời gian tính bằng giây, dùng cho Tie-breaker

        // Thuộc tính điều hướng
        public virtual TranDauTrucTiep TranDau { get; set; } = null!;
        public virtual CauHoi CauHoi { get; set; } = null!;
        public virtual NguoiDung User { get; set; } = null!;
    }
}