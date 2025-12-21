using QUIZ_GAME_WEB.Models.ResultsModels;
using System;
using System.Collections.Generic;
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class CauHoi
    {
        [Key]
        public int CauHoiID { get; set; }

        // ======================= KHÓA NGOẠI =======================
        [Required]
        [ForeignKey(nameof(ChuDe))]
        public int ChuDeID { get; set; }

        [Required]
        [ForeignKey(nameof(DoKho))]
        public int DoKhoID { get; set; }

        [ForeignKey(nameof(QuizTuyChinh))]
        public int? QuizTuyChinhID { get; set; }

        [ForeignKey(nameof(AdminDuyet))]
        public int? AdminDuyetID { get; set; }

        // ======================= NỘI DUNG =======================
        [Required, MaxLength(500)]
        public string NoiDung { get; set; } = null!;

        [Required, MaxLength(255)]
        public string DapAnA { get; set; } = null!;

        [Required, MaxLength(255)]
        public string DapAnB { get; set; } = null!;

        [Required, MaxLength(255)]
        public string DapAnC { get; set; } = null!;

        [Required, MaxLength(255)]
        public string DapAnD { get; set; } = null!;

        [Required, MaxLength(10)]
        public string DapAnDung { get; set; } = null!;

        [MaxLength(255)]
        public string? HinhAnh { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string TrangThaiDuyet { get; set; } = "Pending";

        // ======================= NAVIGATION (IGNORE ĐỂ CHỐNG LOOP) =======================

        // CHỐT QUAN TRỌNG: tất cả navigation 1-n & n-1 phải JsonIgnore
        // vì DTO đã handle việc lấy thông tin.

        [JsonIgnore]
        public virtual ChuDe ChuDe { get; set; } = null!;

        [JsonIgnore]
        public virtual DoKho DoKho { get; set; } = null!;

        [JsonIgnore]
        public virtual NguoiDung? AdminDuyet { get; set; }

        [JsonIgnore]
        public virtual QuizTuyChinh? QuizTuyChinh { get; set; }

        [JsonIgnore]
        public virtual ICollection<CauSai> CauSais { get; set; } = new HashSet<CauSai>();

        [JsonIgnore]
        public virtual ICollection<TranDauCauHoi> TranDauCauHois { get; set; } = new HashSet<TranDauCauHoi>();

        // ======================= NOT MAPPED =======================

        [NotMapped]
        public string? CacLuaChon { get; set; }
    }
}
