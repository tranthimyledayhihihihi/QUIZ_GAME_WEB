using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.ViewModels;

namespace WEBB.Controllers.User
{
    [Authorize]
    public class GiftController : Controller
    {
        // API bạn đang dùng trên Swagger: /api/user/Achievement/...
        private readonly string API_BASE = "https://localhost:7092/api/user/Achievement";

        // =========================
        // GET: /User/Gift
        // View: ~/Views/Menu/Gifts.cshtml
        // =========================
        public async Task<ActionResult> Index()
        {
            var model = new DailyRewardPageViewModel();
            model.Message = TempData["Message"] as string ?? string.Empty;

            using (var client = CreateClient())
            {
                // 1) STREAK
                try
                {
                    var streakRes = await client.GetAsync($"{API_BASE}/streak");
                    if (streakRes.IsSuccessStatusCode)
                    {
                        var streakJson = await streakRes.Content.ReadAsStringAsync();

                        // Trường hợp API trả camelCase: soNgayLienTiep, ngayCapNhatCuoi
                        dynamic streak = JsonConvert.DeserializeObject(streakJson);

                        // Nếu API bạn không có streak endpoint thì đoạn này sẽ fail -> bỏ qua
                        model.SoNgayLienTiep = streak.soNgayLienTiep != null ? (int)streak.soNgayLienTiep : 0;
                    }
                }
                catch
                {
                    // Không có streak endpoint hoặc lỗi -> vẫn cho trang chạy
                    model.SoNgayLienTiep = 0;
                }

                // 2) MY REWARDS (JSON bạn gửi)
                try
                {
                    var rewardsRes = await client.GetAsync($"{API_BASE}/my-rewards");
                    if (rewardsRes.IsSuccessStatusCode)
                    {
                        var rewardsJson = await rewardsRes.Content.ReadAsStringAsync();

                        // API trả dạng: thuongID, userID, ngayNhan...
                        var rewards = JsonConvert.DeserializeObject<List<RewardItemViewModel>>(rewardsJson);
                        if (rewards != null)
                            model.Rewards = rewards;
                    }
                }
                catch
                {
                    // lỗi parse -> để danh sách rỗng
                    model.Rewards = new List<RewardItemViewModel>();
                }
            }

            return View("~/Views/Menu/Gifts.cshtml", model);
        }

        // =========================
        // POST: /User/Gift/ClaimDailyReward
        // =========================
        [HttpPost]
        public async Task<ActionResult> ClaimDailyReward()
        {
            using (var client = CreateClient())
            {
                var res = await client.PostAsync($"{API_BASE}/daily-reward", null);
                var json = await res.Content.ReadAsStringAsync();

                // API thường trả: { message: "..." } hoặc { Message: "..." }
                try
                {
                    dynamic result = JsonConvert.DeserializeObject(json);
                    TempData["Message"] = result.message != null ? result.message.ToString()
                                     : result.Message != null ? result.Message.ToString()
                                     : "Đã xử lý yêu cầu.";
                }
                catch
                {
                    TempData["Message"] = res.IsSuccessStatusCode
                        ? "Nhận thưởng thành công."
                        : "Không thể nhận thưởng.";
                }
            }

            return RedirectToAction("Index");
        }

        // =========================
        // JWT CLIENT
        // =========================
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var token = Session["JWT_TOKEN"]?.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}
