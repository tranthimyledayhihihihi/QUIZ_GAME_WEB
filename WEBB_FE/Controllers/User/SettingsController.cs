using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WEBB.Models.User;

namespace WEBB.Controllers.User
{
    public class SettingsController : Controller
    {
        // API backend
        private readonly string _apiBaseUrl = "https://localhost:5001";

        private string GetToken()
        {
            // Ưu tiên Session
            var token = Session["JwtToken"] as string;

            // Nếu Session chưa có, thử đọc Cookie
            if (string.IsNullOrEmpty(token) && Request.Cookies["JwtToken"] != null)
            {
                token = Request.Cookies["JwtToken"].Value;
            }

            return token;
        }

        // =========================
        // GET: /User/Settings
        // =========================
        public async Task<ActionResult> Index()
        {
            var token = GetToken();

            // ❗ Không redirect nữa — nếu chưa login thì chỉ báo lỗi nhẹ và không gọi API
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập — vui lòng login để xem cài đặt.";
                return View(new SettingDto()); // trả về model rỗng để không bị crash
            }

            var model = new SettingDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync("api/user/profile/me");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    if (data.caiDat != null)
                    {
                        model.AmThanh = data.caiDat.amThanh ?? true;
                        model.NhacNen = data.caiDat.nhacNen ?? true;
                        model.ThongBao = data.caiDat.thongBao ?? true;
                        model.NgonNgu = data.caiDat.ngonNgu ?? "vi";
                    }
                }
                else
                {
                    TempData["Error"] = "Không lấy được cài đặt từ server (có thể token hết hạn).";
                }
            }

            return View(model);
        }

        // =========================
        // POST: /User/Settings
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(SettingDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = GetToken();

            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập — không thể cập nhật cài đặt.";
                return View(model);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var jsonBody = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("api/user/profile/settings", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật cài đặt thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Cập nhật thất bại: " + error);
                }
            }

            return View(model);
        }
    }
}
