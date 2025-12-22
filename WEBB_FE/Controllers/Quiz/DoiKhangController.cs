using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WEBB.Models.Quiz;
using WEBB.ViewModels;

namespace WEBB.Controllers.Quiz
{
    public class DoiKhangController : Controller
    {
        private readonly string _apiBase;

        public DoiKhangController()
        {
            _apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(_apiBase))
                throw new InvalidOperationException("Thiếu cấu hình ApiBaseUrl (ApiBaseUrl).");
            if (!_apiBase.EndsWith("/")) _apiBase += "/";
        }

        private string GetToken()
        {
            var token = Session["JwtToken"] as string;
            if (string.IsNullOrEmpty(token))
                token = Session["JWT_TOKEN"] as string;
            return token;
        }

        // GET: /Quiz/DoiKhang
        public ActionResult Index()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var vm = new DuelLobbyViewModel();
            return View("~/Views/User/Quiz/DoiKhang/Index.cshtml", vm);
        }

        // POST: /Quiz/DoiKhang/FindMatch
        [HttpPost]
        public async Task<ActionResult> FindMatch()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                try
                {
                    // TODO: route BE tìm trận, ví dụ: api/duel/find
                    var res = await client.PostAsync("api/duel/find", null);
                    if (!res.IsSuccessStatusCode)
                        return Json(new { success = false, message = "Không tìm được trận." });

                    var json = await res.Content.ReadAsStringAsync();
                    var duel = JsonConvert.DeserializeObject<DuelMatchDto>(json);

                    return Json(new { success = true, data = duel });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
        // GET: /Quiz/DoiKhang/Create
        public ActionResult Create()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            // Dùng chung Index (JS sẽ gửi CREATE_ROOM qua WebSocket)
            ViewBag.Mode = "CREATE";
            return View("~/Views/User/Quiz/DoiKhang/Create.cshtml");
        }

        // GET: /Quiz/DoiKhang/Join?code=ABC123
        public ActionResult Join(string code)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            ViewBag.Mode = "JOIN";
            ViewBag.RoomCode = code; // đổ sẵn mã phòng nếu có
            return View("~/Views/User/Quiz/DoiKhang/Join.cshtml");
        }

        public ActionResult Match(string code)
        {
            var token = GetToken(); // dùng lại hàm bạn đã có trong controller
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            ViewBag.MatchCode = code;
            return View("~/Views/User/Quiz/DoiKhang/Match.cshtml");
        }

    }
}