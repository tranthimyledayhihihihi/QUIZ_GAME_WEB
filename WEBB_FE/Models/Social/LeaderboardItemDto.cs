namespace WEBB.Models.Social
{
    public class LeaderboardItemDto
    {
        public int UserID { get; set; }
        public string TenHienThi { get; set; }
        public string AnhDaiDien { get; set; }

        public int HangThang { get; set; }
        public int DiemThang { get; set; }

        public int HangNam { get; set; }
        public int DiemNam { get; set; }

        public bool IsOnline { get; set; }
    }
}
