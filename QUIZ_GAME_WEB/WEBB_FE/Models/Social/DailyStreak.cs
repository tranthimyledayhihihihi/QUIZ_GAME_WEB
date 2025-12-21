// WEBB/Models/DailyStreak.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBB.Models
{
    [Table("DailyStreaks")]
    public class DailyStreak
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        public int SoNgayLienTiep { get; set; }

        public DateTime? NgayCapNhatCuoi { get; set; }

        public bool DaNhanThuongHomNay { get; set; }

        public int TongDiemDaNhan { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    [Table("DailyStreakHistories")]
    public class DailyStreakHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        public DateTime CheckInDate { get; set; }

        public int DiemThuong { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    [Table("StreakBadges")]
    public class StreakBadge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Icon { get; set; }

        public int RequiredDays { get; set; }

        public int RewardPoints { get; set; }
    }

    [Table("UserBadges")]
    public class UserBadge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        public int BadgeId { get; set; }

        public DateTime EarnedAt { get; set; }
    }
}