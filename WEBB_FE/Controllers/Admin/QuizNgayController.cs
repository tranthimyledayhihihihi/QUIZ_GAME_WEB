using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class QuizNgayController : Controller
    {
        private readonly string apiBase =
            "https://localhost:7092/api/admin/quiz-ngay";

        // =====================================
        // GET: /Admin/QuizNgay
        // =====================================
        public async Task<ActionResult> Index(int month = 0, int year = 0)
        {
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            ViewBag.Month = month;
            ViewBag.Year = year;

            var list = new List<QuizNgayDto>();

            using (var client = CreateClient())
            {
                var res = await client.GetAsync(
                    $"{apiBase}/lich-trinh?month={month}&year={year}");

                if (res.IsSuccessStatusCode)
                {
                    list = JsonConvert.DeserializeObject<List<QuizNgayDto>>(
                        await res.Content.ReadAsStringAsync());
                }
                else
                {
                    ViewBag.Error = "Không thể tải lịch Quiz Ngày";
                }
            }

            return View("~/Views/Admin/QuizNgay/Index.cshtml", list);
        }

        // =====================================
        // POST: SET DAILY QUIZ
        // =====================================
        [HttpPost]
        public async Task<ActionResult> SetDaily(DateTime ngay, int cauHoiID)
        {
            using (var client = CreateClient())
            {
                var payload = new
                {
                    Ngay = ngay,
                    CauHoiID = cauHoiID
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = await client.PostAsync($"{apiBase}/set-daily", content);

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Thiết lập Quiz Ngày thất bại";
                }
                else
                {
                    TempData["Success"] = "Đã thiết lập Quiz Ngày thành công";
                }
            }

            return RedirectToAction("Index",
                new { month = ngay.Month, year = ngay.Year });
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
