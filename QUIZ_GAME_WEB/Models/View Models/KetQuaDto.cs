// Ví dụ: Models/ViewModels/KetQuaDto.cs
public class KetQuaDto
{
    public int QuizAttemptID { get; set; }
    public int Diem { get; set; }
    public int SoCauDung { get; set; }
    public int TongCauHoi { get; set; }
    public string TrangThaiKetQua { get; set; } = null!;
    // Không bao gồm QuizAttempt hoặc các thuộc tính điều hướng khác.
}