using System.Collections.Generic;
using WEBB.Models.Social;

namespace WEBB.Models.ViewModels
{
    public class LeaderboardViewModel
    {
        public string Type { get; set; } = "monthly";
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalUsers { get; set; }

        public List<LeaderboardItemDto> Items { get; set; } = new List<LeaderboardItemDto>();
    }
}
