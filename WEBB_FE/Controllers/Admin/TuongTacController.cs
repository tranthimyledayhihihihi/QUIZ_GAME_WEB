using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class TuongTacController : Controller
    {
        private readonly string API_URL =
            "https://localhost:7092/api/admin/tuong-tac";

        // ===============================
        // DANH SÁCH BÌNH LUẬN
        // ===============================
        public async Task<ActionResult> Index(int page = 1)
        {
            var list = new List<TuongTacDto>();

            using (var client = CreateClient())
            {
                var res = await client.GetAsync($"{API_URL}/binh-luan?page={page}&pageSize=20");

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(json);

                    list = JsonConvert.DeserializeObject<List<TuongTacDto>>(
                        JsonConvert.SerializeObject(result.data)
                    );
                }
                else
                {
                    ViewBag.Error = "Không thể tải danh sách bình luận";
                }
            }

            return View("~/Views/Admin/TuongTac/Index.cshtml", list);
        }

        // ===============================
        // XÓA BÌNH LUẬN (SUPERADMIN)
        // ===============================
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            using (var client = CreateClient())
            {
                await client.DeleteAsync($"{API_URL}/binh-luan/{id}");
            }
            return RedirectToAction("Index");
        }

        // ===============================
        // HTTP CLIENT + JWT
        // ===============================
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
        // ===============================
        // LỊCH SỬ CHIA SẺ QUIZ
        // ===============================
        public async Task<ActionResult> LichSuChiaSe()
        {
            using (var client = CreateClient())
            {
                var res = await client.GetAsync($"{API_URL}/lich-su-chia-se");

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    ViewBag.RawData = json; // tạm thời hiển thị thô
                }
                else
                {
                    ViewBag.Error = "Không thể tải lịch sử chia sẻ";
                }
            }

            return View("~/Views/Admin/TuongTac/LichSuChiaSe.cshtml");
        }

        


    }
}
