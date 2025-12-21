// Models/Implementations/RewardService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Threading.Tasks;
using System; // 👈 ĐÃ THÊM: Cần cho DateTime
using System.Collections.Generic; // 👈 ĐÃ THÊM: Cần cho IEnumerable

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class RewardService : IRewardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RewardService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId)
        {
            // TỐT NHẤT: Gọi qua IAchievementsRepository (giả định đã được khai báo trong IUnitOfWork)
            // Nếu bạn chưa thêm IAchievementsRepository vào IUnitOfWork, hãy gọi:
            // return await _unitOfWork.Achievements.GetAchievementsByUserIdAsync(userId); 

            // Hiện tại, giữ nguyên theo IResultRepository của bạn (cho đến khi bạn tách)
            var userAchievements = await _unitOfWork.Results.GetUserAchievementsAsync(userId);
            return userAchievements;
        }

        public async Task<bool> CheckAndAwardDailyRewardAsync(int userId)
        {
            var today = DateTime.Now.Date;

            // Lỗi biên dịch được khắc phục bằng cách sửa Interface
            var rewardReceived = await _unitOfWork.Results.GetDailyRewardByDateAsync(userId, today);

            if (rewardReceived != null)
            {
                return false; // Đã nhận rồi
            }

            // Logic 2: Trao thưởng (Thêm bản ghi ThuongNgay vào DB)
            var newReward = new ThuongNgay
            {
                UserID = userId,
                NgayNhan = today,
                PhanThuong = "100 điểm",
                DiemThuong = 100,
                TrangThaiNhan = true
            };

            _unitOfWork.Results.AddDailyReward(newReward);
            await _unitOfWork.CompleteAsync();

            // Logic 3: Cập nhật Streak (Chuỗi Ngày)
            await UpdateUserStreak(userId);

            return true;
        }

        private async Task UpdateUserStreak(int userId)
        {
            var streak = await _unitOfWork.Results.GetUserStreakAsync(userId);
            var today = DateTime.Now.Date;

            if (streak == null)
            {
                // Tạo mới streak
                _unitOfWork.Results.AddStreak(new ChuoiNgay { UserID = userId, SoNgayLienTiep = 1, NgayCapNhatCuoi = DateTime.Now });
            }
            else if (streak.NgayCapNhatCuoi.Date == today.AddDays(-1))
            {
                // Tiếp tục streak
                streak.SoNgayLienTiep++;
                streak.NgayCapNhatCuoi = DateTime.Now;
                _unitOfWork.Results.Update(streak); // Dùng Generic Update (Đã có trong IResultRepository)
            }
            // Không cần else: Nếu bị đứt chuỗi, logic reset (NgayCapNhatCuoi != today) sẽ được xử lý tại thời điểm check streak tiếp theo.
        }
    }
}