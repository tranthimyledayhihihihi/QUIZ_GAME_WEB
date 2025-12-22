using System.Collections.Generic;

namespace WEBB.Models.ViewModels
{
    public class DailyRewardPageViewModel
    {
        // Streak
        public int SoNgayLienTiep { get; set; }
        public string Message { get; set; }

        // Rewards list
        public List<RewardItemViewModel> Rewards { get; set; }

        public DailyRewardPageViewModel()
        {
            Rewards = new List<RewardItemViewModel>();
            Message = string.Empty;
        }
    }
}
