using System;
using System.Configuration;
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
        private readonly string _apiBase;

        public SettingsController()
        {
            _apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(_apiBase))
                throw new InvalidOperationException("Thiếu cấu hình ApiBaseUrl trong Web.config (key: ApiBaseUrl).");

            if (!_apiBase.EndsWith("/"))
                _apiBase += "/";
        }

        private string GetToken()
        {
            var token = Session["JwtToken"] as string;

            if (string.IsNullOrEmpty(token))
                token = Session["JWT_TOKEN"] as string; // backup nếu bạn lỡ lưu key này

            if (string.IsNullOrEmpty(token) && Request.Cookies["JwtToken"] != null)
                token = Request.Cookies["JwtToken"].Value;

            return token;
        }

        // =========================
        // GET: /User/Settings
        // =========================
        public async Task<ActionResult> Index()
        {
            var token = GetToken();

            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập — vui lòng login để xem cài đặt.";
                return View(new SettingDto());
            }

            var model = new SettingDto();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync("api/user/profile/me");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không kết nối được tới API (Settings): " + ex.Message;
                    return View(model);
                }

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
                    TempData["Error"] = "Không lấy được cài đặt từ server. Mã lỗi: " +
                                        (int)response.StatusCode + " - " + response.StatusCode;
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
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var jsonBody = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await client.PutAsync("api/user/profile/settings", content);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không kết nối được tới API (Settings - PUT): " + ex.Message;
                    return View(model);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật cài đặt thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Cập nhật thất bại. Mã lỗi: " +
                                                 (int)response.StatusCode + " - " + response.StatusCode +
                                                 ". Chi tiết: " + errorText);
                }
            }

            return View(model);
        }
    }
}
