using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB_FE.Models.Admin;

namespace WEBB_FE.Controllers.Admin
{
    public class ThongKeController : Controller
    {
        private readonly string API_BASE = "https://localhost:7092/api/ThongKe";

        // =====================================
        // GET: /Admin/ThongKe?userId=2
        // =====================================
        public async Task<ActionResult> Index(int userId = 0)
        {
            var model = new ThongKeViewModel();

            if (userId <= 0)
            {
                ViewBag.Error = "Vui lòng chọn UserID để xem thống kê";
                return View("~/Views/Admin/ThongKe/Index.cshtml", model);
            }

            model.UserID = userId;

            using (var client = new HttpClient())
            {
                // -------------------------------
                // 1. DAILY STATS (30 ngày)
                // -------------------------------
                var dailyRes = await client.GetAsync($"{API_BASE}/User/{userId}/Daily");
                if (dailyRes.IsSuccessStatusCode)
                {
                    model.DailyStats = JsonConvert.DeserializeObject<List<ThongKeDailyDto>>(
                        await dailyRes.Content.ReadAsStringAsync()
                    );
                }

                // -------------------------------
                // 2. STREAK
                // -------------------------------
                var streakRes = await client.GetAsync($"{API_BASE}/User/{userId}/Streak");
                if (streakRes.IsSuccessStatusCode)
                {
                    model.Streak = JsonConvert.DeserializeObject<ThongKeStreakDto>(
                        await streakRes.Content.ReadAsStringAsync()
                    );
                }
            }

            return View("~/Views/Admin/ThongKe/Index.cshtml", model);
        }
    }
}
