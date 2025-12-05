using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WEBB.Models.Quiz;
using WEBB.Models.ViewModels;
using WEBB.Models.Quiz;



namespace WEBB.Controllers.Quiz
{
    public class ArenaController : Controller
    {
        private readonly string _apiBase;

        public ArenaController()
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

        // GET: /Quiz/Arena
        public async Task<ActionResult> Index()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var vm = new ArenaViewModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage res;
                try
                {
                    // TODO: đổi route này cho khớp BE. Ví dụ: api/arena/rooms
                    res = await client.GetAsync("api/arena/rooms");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không kết nối được tới API: " + ex.Message;
                    return View(vm);
                }

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    // TODO: nếu BE trả format khác thì sửa mapping
                    vm.Rooms = JsonConvert.DeserializeObject<System.Collections.Generic.List<ArenaRoomDto>>(json);
                }
                else
                {
                    TempData["Error"] = $"Không tải được danh sách phòng. Mã lỗi: {(int)res.StatusCode}";
                }
            }

            return View(vm);
        }

        // POST: /Quiz/Arena/CreateRoom
        [HttpPost]
        public async Task<ActionResult> CreateRoom(string roomName)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var payload = new { name = roomName };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                try
                {
                    // TODO: route BE tạo phòng, ví dụ: api/arena/createroom
                    var res = await client.PostAsync("api/arena/createroom", content);

                    if (!res.IsSuccessStatusCode)
                        return Json(new { success = false, message = "Tạo phòng thất bại." });

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        // POST: /Quiz/Arena/Join
        [HttpPost]
        public async Task<ActionResult> Join(int? roomId)
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
                    // TODO: route join phòng, ví dụ: api/arena/join/{id}
                    var res = await client.PostAsync($"api/arena/join/{roomId}", null);

                    if (!res.IsSuccessStatusCode)
                        return Json(new { success = false, message = "Không thể tham gia phòng." });

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }
}
