// Models/Implementations/ReportingService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ReportingService : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportingService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<BaoCaoModel> GenerateReportAsync(BaoCaoRequestModel request)
        {
            // Lấy dữ liệu thống kê theo ngày (ThongKeNguoiDung Entity)
            var stats = await _unitOfWork.Results.GetUserDailyStatsAsync(request.UserID, request.NgayBatDau, request.NgayKetThuc);

            // Logic tính toán tổng hợp
            int tongSoTran = stats.Sum(s => s.SoTran);
            float tyLeDungTB = tongSoTran > 0 ? stats.Sum(s => s.SoCauDung) / (tongSoTran * 5f) : 0; // Giả định mỗi trận 5 câu

            return new BaoCaoModel
            {
                TieuDe = $"Báo cáo hoạt động từ {request.NgayBatDau:dd/MM} đến {request.NgayKetThuc:dd/MM}",
                TongSoLanChoi = tongSoTran,
                TyLeTraLoiDungTrungBinh = tyLeDungTB,
                // Ánh xạ chi tiết (ChiTietNgayModel phải có sẵn)
                ChiTietTheoNgay = stats.Select(s => NewMethod(s)).ToList()
            };
        }

        private static ChiTietNgayModel NewMethod(ResultsModels.ThongKeNguoiDung s)
        {
            return new ChiTietNgayModel
            {
                Ngay = s.Ngay,
                DiemDatDuoc = (float)s.DiemTrungBinh,
                SoLuotChoi = s.SoTran
            };
        }

        public Task<ThongKeViewModel> GetUserPerformanceStatsAsync(int? chuDeId)
        {
            // Logic: Phân tích hiệu suất người dùng (Ví dụ: So sánh tỉ lệ đúng của user với TB cộng đồng)
            return Task.FromResult(new ThongKeViewModel());
        }
    }
}
