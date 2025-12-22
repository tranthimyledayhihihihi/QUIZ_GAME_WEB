using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        private class LoginApiResponse
        {
            public string Token { get; set; }
            public string HoTen { get; set; }
            public string VaiTro { get; set; }
        }

        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/User/Account/Login.cshtml", new LoginRequest());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);

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
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();

                // Nếu API trả HTTP lỗi (400, 401, 500...)
                if (!response.IsSuccessStatusCode)
                {
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

                    return View(model);
                }

                // Parse JSON kết quả trả về
                LoginApiResponse result = JsonConvert.DeserializeObject<LoginApiResponse>(json);

                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    model.ErrorMessage = "Đăng nhập thất bại (token rỗng).";
                    return View(model);
                }

                // Lưu token vào Session và vai trò của người dùng
                Session["JWT_TOKEN"] = result.Token;
                Session["Role"] = result.VaiTro;  // Lưu vai trò của người dùng
                FormsAuthentication.SetAuthCookie(model.UserName, false);

                // Kiểm tra vai trò và chuyển hướng đến layout Admin nếu là Admin hoặc Moderator
                if (result.VaiTro == "SuperAdmin" || result.VaiTro == "Moderator")
                {
                    // Chuyển hướng đến Admin/Topics
                    return RedirectToAction("Index", "Topics");  // Chuyển hướng tới controller Topics mà không cần dùng area
                }
                else
                {
                    return RedirectToAction("Index", "Home");  // Chuyển hướng tới trang chính cho người dùng
                }
            }
        }

        // GET: /Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View("~/Views/User/Account/Register.cshtml", new RegisterRequest());
        }

        // POST: /Account/Register
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
                    xacNhanMatKhau = model.ConfirmPassword,
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
                        model.ErrorMessage = "Đăng ký thất bại.";
                    }

                    return View("~/Views/User/Account/Register.cshtml", model);
                }

                TempData["RegisterSuccess"] = "Đăng ký thành công, hãy đăng nhập.";
                return RedirectToAction("Login");
            }
        }

        // LOGOUT
        public async Task<ActionResult> Logout()
        {
            string token = Session["JWT_TOKEN"] as string;

            if (!string.IsNullOrEmpty(token))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBase);

                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    try
                    {
                        // 🔥 GỌI API LOGOUT BE
                        await client.PostAsync("api/account/logout", null);
                    }
                    catch
                    {
                        // Không cần xử lý gì – dù BE fail vẫn logout FE
                    }
                }
            }

            // 🔐 Clear FE session
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }

    }
}
