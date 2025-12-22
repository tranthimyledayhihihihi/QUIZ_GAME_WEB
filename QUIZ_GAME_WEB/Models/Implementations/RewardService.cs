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
            bool isUpdated = false;
            // 1. LUÔN CẬP NHẬT STREAK TRƯỚC (Không bị chặn bởi Reward)
            var streak = await _unitOfWork.Results.GetUserStreakAsync(userId);
            if (streak == null || streak.NgayCapNhatCuoi.Date < today)
            {
                await UpdateUserStreak(userId); // Hàm này có await CompleteAsync ở trong rồi
                isUpdated = true;
            }
            // 2. KIỂM TRA VÀ TRAO THƯỞNG (REWARD)
            var rewardReceived = await _unitOfWork.Results.GetDailyRewardByDateAsync(userId, today);
            if (rewardReceived == null)
            {
                var newReward = new ThuongNgay
                {
                    UserID = userId,
                    NgayNhan = today,
                    PhanThuong = "100 điểm",
                    DiemThuong = 100,
                    TrangThaiNhan = true
                };
                _unitOfWork.Results.AddDailyReward(newReward);

                // Lưu phần thưởng
                await _unitOfWork.CompleteAsync();
                isUpdated = true;
            }
            return isUpdated; // Trả về true nếu CÓ cập nhật bất kỳ cái nào (Streak hoặc Reward)
        }
        private async Task UpdateUserStreak(int userId)
        {
            var streak = await _unitOfWork.Results.GetUserStreakAsync(userId);
            var today = DateTime.Today;
            if (streak == null)
            {
                _unitOfWork.Results.AddStreak(new ChuoiNgay
                {
                    UserID = userId,
                    SoNgayLienTiep = 1,
                    NgayCapNhatCuoi = DateTime.Now
                });
            }
            else
            {
                // Nếu hôm qua điểm danh -> tăng chuỗi
                if (streak.NgayCapNhatCuoi.Date == today.AddDays(-1))
                {
                    streak.SoNgayLienTiep++;
                }
                // Nếu bỏ lỡ quá 1 ngày -> reset về 1
                else if (streak.NgayCapNhatCuoi.Date < today.AddDays(-1))
                {
                    streak.SoNgayLienTiep = 1;
                }
                streak.NgayCapNhatCuoi = DateTime.Now;
                _unitOfWork.Results.Update(streak);
            }
            // QUAN TRỌNG: Phải có dòng này để lưu vào Database
            await _unitOfWork.CompleteAsync();
        }
    }
}