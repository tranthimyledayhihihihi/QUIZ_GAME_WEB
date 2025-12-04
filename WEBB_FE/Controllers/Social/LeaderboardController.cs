// WEBB/Controllers/Social/LeaderboardController.cs
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WEBB.Models.Social;
using WEBB.Models.ViewModels;

namespace WEBB.Controllers.Social
{
    public class LeaderboardController : Controller
    {
        private readonly string _apiBase;

        public LeaderboardController()
        {
            _apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7180/";
            if (!_apiBase.EndsWith("/")) _apiBase += "/";
        }

        // Lớp dùng để đọc JSON trả về từ API Ranking
        private class LeaderboardApiResponse
        {
            public string Type { get; set; }
            public int TongSoNguoi { get; set; }
            public int TrangHienTai { get; set; }
            public int TongSoTrang { get; set; }
            public LeaderboardItemDto[] DanhSach { get; set; }
        }

        // GET: /Leaderboard?type=weekly|monthly&page=1
        public async Task<ActionResult> Index(string type = "monthly", int page = 1, int pageSize = 10)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);

                // BXH cho phép xem public nên không bắt buộc JWT
                var url = $"api/Ranking/leaderboard?type={type}&pageNumber={page}&pageSize={pageSize}";
                var res = await client.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    // Lỗi API -> trả về view rỗng
                    var emptyModel = new LeaderboardViewModel
                    {
                        Type = type,
                        CurrentPage = 1,
                        TotalPages = 1,
                        TotalUsers = 0
                    };
                    ViewBag.Error = "Không lấy được dữ liệu xếp hạng.";
                    return View(emptyModel);
                }

                var json = await res.Content.ReadAsStringAsync();
                var apiData = JsonConvert.DeserializeObject<LeaderboardApiResponse>(json);

                var model = new LeaderboardViewModel
                {
                    Type = apiData.Type,
                    CurrentPage = apiData.TrangHienTai,
                    TotalPages = apiData.TongSoTrang,
                    TotalUsers = apiData.TongSoNguoi,
                    Items = new System.Collections.Generic.List<LeaderboardItemDto>(apiData.DanhSach)
                };

                return View(model);
            }
        }
    }
}
