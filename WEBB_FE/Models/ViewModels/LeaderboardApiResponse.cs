using System.Collections.Generic;
using WEBB.Models.Social;

namespace WEBB.Models.ViewModels
{
    public class LeaderboardApiResponse
    {
        public string Type { get; set; }
        public int TongSoNguoi { get; set; }
        public int TrangHienTai { get; set; }
        public int TongSoTrang { get; set; }
        public List<LeaderboardItemDto> DanhSach { get; set; }
    }
}
