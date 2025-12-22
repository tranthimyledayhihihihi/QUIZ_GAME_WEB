using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class SessionsController : Controller
    {
        private readonly string apiBase = "https://localhost:7092/api/admin/phiendangnhap";

        // ==================================================
        // INDEX + SEARCH + FILTER
        // ==================================================
        public async Task<ActionResult> Index(
            int page = 1,
            int pageSize = 20,
            string keyword = null,
            bool? isActive = null)
        {
            using (var client = CreateClient())
            {
                var url = $"{apiBase}?page={page}&pageSize={pageSize}&keyword={keyword}&isActive={isActive}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không lấy được danh sách phiên đăng nhập!";
                    return View("~/Views/Admin/Sessions/Index.cshtml",
                        new List<AdminSessionDto>());
                }

                var json = await response.Content.ReadAsStringAsync();

                // ✅ DESERIALIZE AN TOÀN – KHÔNG dynamic
                var result = JsonConvert.DeserializeObject<AdminSessionResponseDto>(json);

                if (result == null || result.Sessions == null)
                {
                    TempData["Error"] = "Dữ liệu phiên đăng nhập không hợp lệ!";
                    return View("~/Views/Admin/Sessions/Index.cshtml",
                        new List<AdminSessionDto>());
                }

                ViewBag.TotalCount = result.TotalCount;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TodayCount = result.TodayCount;

                return View("~/Views/Admin/Sessions/Index.cshtml", result.Sessions);
            }
        }

        // ==================================================
        // DETAILS
        // ==================================================
        public async Task<ActionResult> Details(int id)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync($"{apiBase}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy phiên đăng nhập!";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var session = JsonConvert.DeserializeObject<AdminSessionDto>(json);

                return View("~/Views/Admin/Sessions/Details.cshtml", session);
            }
        }

        // ==================================================
        // FORCE LOGOUT (SuperAdmin)
        // ==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForceLogout(int id)
        {
            using (var client = CreateClient())
            {
                var response = await client.PostAsync(
                    $"{apiBase}/buoc-dang-xuat/{id}", null);

                var msg = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    TempData["Error"] = msg;
                else
                    TempData["Success"] = "Buộc đăng xuất thành công!";

                return RedirectToAction("Index");
            }
        }


        // ==================================================
        // JWT CLIENT
        // ==================================================
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
