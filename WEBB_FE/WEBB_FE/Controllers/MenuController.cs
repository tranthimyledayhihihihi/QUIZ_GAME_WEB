using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Quiz;
using WEBB.Models.Social;
using WEBB.Models.ViewModels;
using WEBB.ViewModels;

namespace WEBB.Controllers
{
    public class MenuController : Controller
    {
        private readonly string _apiBase;

        public MenuController()
        {
            _apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7180/";
            if (!_apiBase.EndsWith("/")) _apiBase += "/";
        }

        private string GetToken()
        {
            // Đúng với cách bạn đang dùng ở AccountController
            return Session["JWT_TOKEN"] as string;
        }

        // ================== THƯỞNG NGÀY ==================
        [HttpGet]
        public async Task<ActionResult> Gifts()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new DailyRewardViewModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // GET api/user/achievement/streak
                var res = await client.GetAsync("api/user/achievement/streak");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    model.SoNgayLienTiep = (int)(data.soNgayLienTiep ?? 0);
                    model.NgayCapNhatCuoi = data.ngayCapNhatCuoi;
                    model.Message = "Chuỗi ngày hiện tại của bạn.";
                }
                else
                {
                    model.Message = "Không lấy được dữ liệu chuỗi ngày.";
                }
            }

            return View(model);
        }

        // POST: /Menu/ClaimDailyReward
        [HttpPost]
        public async Task<ActionResult> ClaimDailyReward()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { awarded = false, message = "Bạn cần đăng nhập." });
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // POST api/user/achievement/daily-reward
                var res = await client.PostAsync("api/user/achievement/daily-reward", null);
                var json = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    return Json(new { awarded = false, message = "Lỗi gọi API: " + json });
                }

                // BE trả { awarded, message }
                return Content(json, "application/json");
            }
        }

        // ================== XẾP HẠNG ==================
        public async Task<ActionResult> Leaderboard(string type = "monthly", int page = 1, int pageSize = 10)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);

                var url = $"api/Ranking/leaderboard?type={type}&pageNumber={page}&pageSize={pageSize}";
                var res = await client.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
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

        private class LeaderboardApiResponse
        {
            public string Type { get; set; }
            public int TongSoNguoi { get; set; }
            public int TrangHienTai { get; set; }
            public int TongSoTrang { get; set; }
            public LeaderboardItemDto[] DanhSach { get; set; }
        }

        // ================== CHUỖI NGÀY ==================
        private class StreakApiResponse
        {
            public int SoNgayLienTiep { get; set; }
            public DateTime? NgayCapNhatCuoi { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult> DailyStreak()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new DailyStreakViewModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // GET api/user/achievement/streak
                var res = await client.GetAsync("api/user/achievement/streak");

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<StreakApiResponse>(json);

                    model.SoNgayLienTiep = data?.SoNgayLienTiep ?? 0;
                    model.NgayCapNhatCuoi = data?.NgayCapNhatCuoi;
                    model.Message = "Chuỗi ngày chơi liên tiếp hiện tại của bạn.";
                }
                else
                {
                    model.SoNgayLienTiep = 0;
                    model.NgayCapNhatCuoi = null;
                    model.Message = "Không lấy được dữ liệu chuỗi ngày.";
                }
            }

            return View(model);
        }

        // =============== ĐỐI KHÁNG ===============
        [HttpGet]
        public ActionResult DoiKhang()
        {
            var vm = new Models.ViewModels.DuelLobbyViewModel
            {
                CurrentMatch = new DuelMatchDto
                {
                    MatchId = "",
                    OpponentName = "",
                    Status = "Idle"
                },
                IsWaiting = false,
                Message = "Đang chờ đối thủ..."
            };

            // TRẢ VỀ VIEW + MODEL, KHÔNG redirect
            return View("~/Views/Quiz/DoiKhang/Index.cshtml", vm);
        }

        // ============= ĐẤU TRƯỜNG =============
        [HttpGet]
        public ActionResult DauTruong()
        {
            var vm = new ArenaViewModel
            {
                // tạm thời để trống, sau này bạn fill bằng API
                Rooms = new List<ArenaRoomDto>()
            };

            return View("~/Views/Quiz/Arena/Index.cshtml", vm);
        }

    }
}
