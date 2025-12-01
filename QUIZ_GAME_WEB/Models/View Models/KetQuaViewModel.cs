using System;
using System.Collections.Generic;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.QuizModels;

namespace QUIZ_GAME_WEB.Models.ViewModels
{
    /// <summary>
    /// Model hiển thị kết quả sau khi chơi Quiz
    /// Chuẩn theo database: KetQua + CauSai
    /// </summary>
    public class KetQuaViewModel
    {
        public int KetQuaID { get; set; }
        public int UserID { get; set; }
        public string TenNguoiDung { get; set; } = null!;

        public int Diem { get; set; }               // Điểm thực tế
        public int SoCauDung { get; set; }         // Số câu trả lời đúng
        public int TongCauHoi { get; set; }        // Tổng số câu hỏi
        public DateTime ThoiGian { get; set; }     // Thời gian kết thúc
        public string TrangThaiKetQua { get; set; } = "Hoàn thành"; // trạng thái

        // Danh sách chi tiết các câu trả lời sai
        public List<CauSaiChiTietModel> ChiTietSai { get; set; } = new List<CauSaiChiTietModel>();
    }

    /// <summary>
    /// Chi tiết từng câu trả lời sai
    /// Chuẩn theo bảng CauSai + CauHoi
    /// </summary>
    public class CauSaiChiTietModel
    {
        public int CauHoiID { get; set; }
        public string NoiDungCauHoi { get; set; } = null!;
        public string DapAnDung { get; set; } = null!;
        public string DapAnDaChon { get; set; } = null!;
    }
}
