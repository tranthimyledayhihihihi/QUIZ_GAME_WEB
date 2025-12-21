// Models/Implementations/AdminDashboardService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Collections.Generic;
using System.Linq; // Cần thiết cho các thao tác LINQ
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardModel> GetDashboardSummaryAsync()
        {
            // Các hàm Count() được gọi từ GenericRepository (hoặc IUserRepository, IResultRepository)
            var totalUsers = await _unitOfWork.Users.CountTotalUsersAsync();
            var totalQuizzes = await _unitOfWork.Quiz.CountAsync(); // Dùng Generic Count
            var totalAttempts = await _unitOfWork.Results.CountAsync();

            // Logic tính toán: Giả lập
            float averageCorrectness = 0.75f;

            // Logic: Lấy Log Hoạt động (Giả sử ISystemRepository có hàm này)
            // var recentLogs = await _unitOfWork.Systems.GetRecentLogsAsync(10); 

            return new AdminDashboardModel
            {
                // Khắc phục lỗi Property Missing
                TongSoNguoiDung = totalUsers,
                TongSoQuizDaTao = totalQuizzes,
                TongSoLuotChoiHomNay = totalAttempts,
                TyLeCauTraLoiDungTrungBinh = averageCorrectness,

                // Khắc phục lỗi LogItemModel/Dictionary
                ThongKeHoatDongThang = new Dictionary<string, int>(), // Khởi tạo để tránh lỗi null
                LogHoatDongGanNhat = new List<LogItemModel>() // Khởi tạo để tránh lỗi null
            };
        }

        public Task<ThongKeViewModel> GetUserActivityStatsAsync()
        {
            // Logic: Lấy dữ liệu thống kê phức tạp (ví dụ: số người dùng online, biểu đồ hoạt động)
            // ...
            return Task.FromResult(new ThongKeViewModel());
        }
    }
}