namespace QUIZ_GAME_WEB.Models.ViewModels
{
    public class CauHoiCreateDto
    {
        public string NoiDung { get; set; } = null!;
        public string DapAnA { get; set; } = null!;
        public string DapAnB { get; set; } = null!;
        public string DapAnC { get; set; } = null!;
        public string DapAnD { get; set; } = null!;

        // Chỉ nhận ký tự A, B, C hoặc D
        public string DapAnDung { get; set; } = null!;

        public int ChuDeID { get; set; }
        public int DoKhoID { get; set; }
        public string? HinhAnh { get; set; }
    }
}