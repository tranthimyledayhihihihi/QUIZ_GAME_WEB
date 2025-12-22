using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using WEBB.Models.User;

namespace WEBB.Controllers.User
{
    public class AccountController : Controller
    {
        private readonly string _apiBase;

        public AccountController()
        {
            _apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrWhiteSpace(_apiBase))
                throw new InvalidOperationException("Thiếu cấu hình ApiBaseUrl trong Web.config (key: ApiBaseUrl).");

            if (!_apiBase.EndsWith("/"))
                _apiBase += "/";
        }

        // DTO dùng để nhận JSON trả về từ API /api/account/login
        private class LoginApiResponse
        {
            public string Token { get; set; }
            public string HoTen { get; set; }
            public string VaiTro { get; set; }
        }

        // ===========================
        // GET: /Account/Login
        // ===========================
        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/User/Account/Login.cshtml", new LoginRequest());
        }

        // ===========================
        // POST: /Account/Login
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/User/Account/Login.cshtml", model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);

                // *** QUAN TRỌNG: map đúng tên field mà API cần ***
                var payload = new
                {
                    tenDangNhap = model.UserName,  // -> TenDangNhap bên API
                    matKhau = model.Password       // -> MatKhau bên API
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync("api/account/login", content);
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = "Không kết nối được tới API: " + ex.Message;
                    return View("~/Views/User/Account/Login.cshtml", model);
                }

                var json = await response.Content.ReadAsStringAsync();

                // Nếu API trả HTTP lỗi (400, 401, 500...)
                if (!response.IsSuccessStatusCode)
                {
                    // API của bạn dùng Unauthorized(...) với { message = "..." }
                    // nên mình cố gắng đọc message ra cho bạn xem
                    try
                    {
                        dynamic errorObj = JsonConvert.DeserializeObject(json);
                        model.ErrorMessage = (string)(errorObj?.message ?? "Đăng nhập thất bại.");
                    }
                    catch
                    {
                        model.ErrorMessage = "Đăng nhập thất bại. Mã lỗi: " +
                                             (int)response.StatusCode + " - " + response.StatusCode;
                    }

                    return View("~/Views/User/Account/Login.cshtml", model);
                }

                // HTTP 200 => parse LoginResponseModel từ API
                LoginApiResponse result = null;
                try
                {
                    result = JsonConvert.DeserializeObject<LoginApiResponse>(json);
                }
                catch
                {
                    model.ErrorMessage = "Không đọc được dữ liệu trả về từ API.";
                    return View("~/Views/User/Account/Login.cshtml", model);
                }

                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    model.ErrorMessage = "Đăng nhập thất bại (token rỗng).";
                    return View("~/Views/User/Account/Login.cshtml", model);
                }

                // ✅ Thành công: Lưu token + cookie đăng nhập
                Session["JWT_TOKEN"] = result.Token;
                // Bạn có thể dùng HoTen hoặc UserName, tùy ý
                FormsAuthentication.SetAuthCookie(model.UserName, false);

                return RedirectToAction("Index", "Home");
            }
        }

        // ===========================
        // GET: /Account/Register
        // ===========================
        [HttpGet]
        public ActionResult Register()
        {
            return View("~/Views/User/Account/Register.cshtml", new RegisterRequest());
        }

        // ===========================
        // POST: /Account/Register
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/User/Account/Register.cshtml", model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);

                var payload = new
                {
                    tenDangNhap = model.UserName,
                    matKhau = model.Password,
                    xacNhanMatKhau = model.ConfirmPassword, // ⭐ THÊM DÒNG NÀY
                    email = model.Email,
                    hoTen = model.UserName
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync("api/account/register", content);
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = "Không kết nối được tới API: " + ex.Message;
                    return View("~/Views/User/Account/Register.cshtml", model);
                }

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        dynamic errorObj = JsonConvert.DeserializeObject(json);
                        model.ErrorMessage = (string)(errorObj?.message ?? "Đăng ký thất bại.");
                    }
                    catch
                    {
                        model.ErrorMessage = "Đăng ký thất bại. Mã lỗi: " +
                                             (int)response.StatusCode + " - " + response.StatusCode;
                    }

                    return View("~/Views/User/Account/Register.cshtml", model);
                }

                TempData["RegisterSuccess"] = "Đăng ký thành công, hãy đăng nhập.";
                return RedirectToAction("Login");
            }
        }


        public ActionResult Logout()
        {
            Session["JWT_TOKEN"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
