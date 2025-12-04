namespace WEBB.Models.Social
{
    public class LeaderboardItemDto
    {
        public int UserID { get; set; }
        public string TenHienThi { get; set; }
        public string AnhDaiDien { get; set; }

        public int HangTuan { get; set; }
        public int DiemTuan { get; set; }

        public int HangThang { get; set; }
        public int DiemThang { get; set; }

        public bool IsOnline { get; set; }
    }
}
