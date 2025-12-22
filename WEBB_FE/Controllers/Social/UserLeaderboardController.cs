using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.ViewModels;
using WEBB.Models.Social;
using System.Collections.Generic;

namespace WEBB.Controllers.Social
{
    [Authorize]

    public class UserLeaderboardController : Controller
    {
        private readonly string _apiBase = "https://localhost:7092/";

        public async Task<ActionResult> Index(string type = "monthly", int page = 1)
        {
            var model = new LeaderboardViewModel
            {
                Type = type,
                CurrentPage = page,
                Items = new List<LeaderboardItemDto>()
            };

            var token = Session["JWT_TOKEN"]?.ToString(); // 🔥 LẤY TOKEN

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);

                // 🔥 BẮT BUỘC: GỬI JWT
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                string url;

                if (type == "yearly")
                {
                    // ✅ API YEARLY CỦA BẠN
                    url = "api/Ranking/leaderboard?type=yearly&pageNumber=2024&pageSize=2025";
                }
                else
                {
                    // ✅ API MONTHLY
                    url = $"api/Ranking/leaderboard?type=monthly&pageNumber={page}&pageSize=12";
                }

                var res = await client.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không tải được bảng xếp hạng";
                    return View("~/Views/User/Menu/UserLeaderboard.cshtml", model);
                }

                var json = await res.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<LeaderboardApiResponse>(json);

                if (data == null)
                {
                    ViewBag.Error = "Dữ liệu API không hợp lệ";
                    return View("~/Views/User/Menu/UserLeaderboard.cshtml", model);
                }

                model.TotalUsers = data.TongSoNguoi;
                model.TotalPages = data.TongSoTrang;
                model.CurrentPage = data.TrangHienTai;
                model.Items = data.DanhSach ?? new List<LeaderboardItemDto>();
            }

            return View("~/Views/User/Menu/UserLeaderboard.cshtml", model);
        }
    }
}
