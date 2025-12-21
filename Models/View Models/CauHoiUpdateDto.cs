namespace QUIZ_GAME_WEB.Models.ViewModels
{
    public class CauHoiUpdateDto
    {
        public int CauHoiID { get; set; }
        public string NoiDung { get; set; } = null!;
        public string DapAnA { get; set; } = null!;
        public string DapAnB { get; set; } = null!;
        public string DapAnC { get; set; } = null!;
        public string DapAnD { get; set; } = null!;
        public string DapAnDung { get; set; } = null!; // A, B, C hoặc D
        public int ChuDeID { get; set; }
        public int DoKhoID { get; set; }
        public string? HinhAnh { get; set; }
    }
}