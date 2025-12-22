using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace WEBB.Controllers.Admin
{
    public class ReportsController : Controller
    {
        private readonly string baseApi = "https://localhost:7092/api/admin/BaoCao";

        // ============================================
        // GET: Admin/Reports
        // ============================================
        public async Task<ActionResult> Index()
        {
            var data = await GetReportData();
            return View("~/Views/Admin/Reports/Index.cshtml", data);
        }

        // ============================================
        // GỌI 3 API RIÊNG & GỘP DATA
        // ============================================
        private async Task<BaoCaoDto> GetReportData()
        {
            var result = new BaoCaoDto
            {
                GamesByDate = new List<GameByDateDto>(),
                UserRegistrationByDate = new List<UserRegisterByDateDto>(),
                TopWrongAnswers = new List<TopWrongAnswerDto>()
            };

            using (var client = CreateClient())
            {
                // 1️⃣ Games By Date
                var gamesRes = await client.GetAsync($"{baseApi}/GamesByDate");
                if (gamesRes.IsSuccessStatusCode)
                {
                    var json = await gamesRes.Content.ReadAsStringAsync();
                    result.GamesByDate =
                        JsonConvert.DeserializeObject<List<GameByDateDto>>(json);
                }

                // 2️⃣ User Registration By Date
                var usersRes = await client.GetAsync($"{baseApi}/UserRegistrationByDate");
                if (usersRes.IsSuccessStatusCode)
                {
                    var json = await usersRes.Content.ReadAsStringAsync();
                    result.UserRegistrationByDate =
                        JsonConvert.DeserializeObject<List<UserRegisterByDateDto>>(json);
                }

                // 3️⃣ Top Wrong Answers
                var wrongRes = await client.GetAsync($"{baseApi}/TopWrongAnswers");
                if (wrongRes.IsSuccessStatusCode)
                {
                    var json = await wrongRes.Content.ReadAsStringAsync();
                    result.TopWrongAnswers =
                        JsonConvert.DeserializeObject<List<TopWrongAnswerDto>>(json);
                }
            }

            return result;
        }

        // ============================================
        // HTTP CLIENT + JWT
        // ============================================
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
    