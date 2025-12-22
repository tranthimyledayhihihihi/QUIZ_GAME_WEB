using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using System.Text.Json.Serialization;
namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        [MaxLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string MatKhau { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(100)]
        public string? HoTen { get; set; }
        [MaxLength(255)]
        public string? AnhDaiDien { get; set; }
        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; } = true;
        // ✅ QUAN TRỌNG: Phải có 2 dòng này mới tạo được cột VaiTroID
        [Required]
        public int VaiTroID { get; set; }
        [ForeignKey(nameof(VaiTroID))]
        public virtual VaiTro VaiTro { get; set; } = null!;
        // Các thuộc tính điều hướng (Giữ nguyên)
        [JsonIgnore] public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        [JsonIgnore] public virtual Admin? Admin { get; set; }
        [JsonIgnore] public virtual CaiDatNguoiDung? CaiDat { get; set; }
        [JsonIgnore] public virtual ICollection<PhienDangNhap> PhienDangNhaps { get; set; } = new List<PhienDangNhap>();
        [JsonIgnore] public virtual ICollection<KetQua> KetQuas { get; set; } = new List<KetQua>();
        [JsonIgnore] public virtual ICollection<ChuoiNgay> ChuoiNgays { get; set; } = new List<ChuoiNgay>();
        [JsonIgnore] public virtual ICollection<BXH> BXHs { get; set; } = new List<BXH>();
        [JsonIgnore] public virtual ICollection<NguoiDungOnline> NguoiDungOnlines { get; set; } = new List<NguoiDungOnline>();
        [JsonIgnore] public virtual ICollection<QuizTuyChinh> QuizTuyChinhs { get; set; } = new List<QuizTuyChinh>();
        [JsonIgnore] public virtual ICollection<CauSai> CauSais { get; set; } = new List<CauSai>();
        [JsonIgnore] public virtual ICollection<QuizChiaSe> QuizChiaSesGui { get; set; } = new List<QuizChiaSe>();
        [JsonIgnore] public virtual ICollection<QuizChiaSe> QuizChiaSesNhan { get; set; } = new List<QuizChiaSe>();
        [JsonIgnore] public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}