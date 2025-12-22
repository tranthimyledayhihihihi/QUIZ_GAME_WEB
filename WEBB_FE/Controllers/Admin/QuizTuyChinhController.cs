using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class QuizTuyChinhController : Controller
    {
        private readonly string API_URL =
            "https://localhost:7092/api/admin/quiz-tuy-chinh";

        // =========================================
        // DANH SÁCH QUIZ TÙY CHỈNH
        // =========================================
        public async Task<ActionResult> Index(int page = 1, string status = null)
        {
            var list = new List<QuizTuyChinhDto>();
            ViewBag.Status = status;

            using (var client = CreateClient())
            {
                var url = $"{API_URL}?page={page}&pageSize=20";
                if (!string.IsNullOrEmpty(status))
                    url += $"&status={status}";

                var res = await client.GetAsync(url);

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(json);

                    list = JsonConvert.DeserializeObject<List<QuizTuyChinhDto>>(
                        JsonConvert.SerializeObject(result.data)
                    );
                }
                else
                {
                    ViewBag.Error = "Không thể tải danh sách Quiz Tùy Chỉnh";
                }
            }

            return View("~/Views/Admin/QuizTuyChinh/Index.cshtml", list);
        }

        // =========================================
        // DUYỆT QUIZ
        // =========================================
        [HttpPost]
        public async Task<ActionResult> Approve(int id)
        {
            using (var client = CreateClient())
            {
                await client.PostAsync($"{API_URL}/{id}/phe-duyet", null);
            }
            return RedirectToAction("Index");
        }

        // =========================================
        // TỪ CHỐI QUIZ
        // =========================================
        [HttpPost]
        public async Task<ActionResult> Reject(int id)
        {
            using (var client = CreateClient())
            {
                await client.PostAsync($"{API_URL}/{id}/tu-choi", null);
            }
            return RedirectToAction("Index");
        }

        // =========================================
        // XÓA QUIZ (SUPERADMIN)
        // =========================================
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = CreateClient())
            {
                await client.DeleteAsync($"{API_URL}/{id}");
            }
            return RedirectToAction("Index");
        }

        // =========================================
        // HTTP CLIENT + JWT
        // =========================================
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var token = Session["JWT_TOKEN"]?.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}
