using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class BXHController : Controller
    {
        private readonly string apiBase = "https://localhost:7092/api/admin/bxh";

        // =====================================
        // GET: /Admin/BXH
        // =====================================
        public async Task<ActionResult> Index(int top = 10)
        {
            var list = new List<BXHDto>();
            ViewBag.Top = top;

            using (var client = CreateClient())
            {
                var res = await client.GetAsync($"{apiBase}/top-global?top={top}");
                if (res.IsSuccessStatusCode)
                {
                    list = JsonConvert.DeserializeObject<List<BXHDto>>(
                        await res.Content.ReadAsStringAsync());
                }
                else
                {
                    ViewBag.Error = "Không thể tải bảng xếp hạng";
                }
            }

            return View("~/Views/Admin/BXH/Index.cshtml", list);
        }

        // =====================================
        // GET: Thống kê hôm nay
        // =====================================
        public async Task<ActionResult> TodayStats()
        {
            using (var client = CreateClient())
            {
                var res = await client.GetAsync($"{apiBase}/today-stats");
                if (!res.IsSuccessStatusCode)
                {
                    ViewBag.StatsError = "Không thể tải thống kê hôm nay";
                }
                else
                {
                    ViewBag.TodayStats = await res.Content.ReadAsStringAsync();
                }
            }

            return PartialView("~/Views/Admin/BXH/_TodayStats.cshtml");
        }

        // =====================================
        // HTTP CLIENT + JWT
        // =====================================
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
