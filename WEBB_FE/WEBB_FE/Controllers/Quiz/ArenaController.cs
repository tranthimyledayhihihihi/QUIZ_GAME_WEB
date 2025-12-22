using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WEBB.Models.Quiz;
using WEBB.Models.ViewModels;

namespace WEBB.Controllers.Quiz
{
    public class ArenaController : Controller
    {
        private readonly string _apiBase;

        public ArenaController()
        {
            _apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7180/";
            if (!_apiBase.EndsWith("/")) _apiBase += "/";
        }

        private string GetToken()
        {
            return Session["JWT_TOKEN"] as string;
        }

        // ============ MÀN DANH SÁCH PHÒNG ============
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account");
            }

            var vm = new ArenaViewModel
            {
                Rooms = new List<ArenaRoomDto>() // Khởi tạo danh sách trống
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                try
                {
                    var res = await client.GetAsync("api/arena/rooms");

                    if (res.IsSuccessStatusCode)
                    {
                        var json = await res.Content.ReadAsStringAsync();
                        vm.Rooms = JsonConvert.DeserializeObject<List<ArenaRoomDto>>(json)
                                   ?? new List<ArenaRoomDto>();
                    }
                    else
                    {
                        TempData["Error"] = "Không thể tải danh sách phòng.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi kết nối API: " + ex.Message;
                }
            }

            return View("~/Views/Quiz/Arena/Index.cshtml", vm);
        }

        // ============ MÀN TẠO PHÒNG ============
        [HttpGet]
        public ActionResult Create()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account");
            }

            // Trả về view tạo phòng
            return View("~/Views/Quiz/Arena/Create.cshtml");
        }

        // POST: /Arena/CreateRoom (AJAX)
        [HttpPost]
        public async Task<ActionResult> CreateRoom(string roomName, int maxPlayers = 4)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var payload = new
                {
                    name = string.IsNullOrEmpty(roomName)
                        ? "Phòng của " + (User.Identity.IsAuthenticated ? User.Identity.Name : "Player")
                        : roomName,
                    maxPlayers = maxPlayers
                };

                var jsonBody = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                try
                {
                    var res = await client.PostAsync("api/arena/create", content);
                    var json = await res.Content.ReadAsStringAsync();

                    if (!res.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Lỗi API: " + json });
                    }

                    // Trả về JSON thành công
                    var result = JsonConvert.DeserializeObject<dynamic>(json);
                    return Json(new
                    {
                        success = true,
                        message = "Tạo phòng thành công!",
                        roomId = result.roomId,
                        code = result.code
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi: " + ex.Message });
                }
            }
        }

        // ============ MÀN JOIN PHÒNG ============
        [HttpGet]
        public ActionResult Join()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account");
            }

            // Trả về view join phòng
            return View("~/Views/Quiz/Arena/Join.cshtml");
        }

        // POST: /Arena/JoinRoom (AJAX)
        [HttpPost]
        public async Task<ActionResult> JoinRoom(string roomCode)
        {
            if (string.IsNullOrEmpty(roomCode))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã phòng." });
            }

            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                try
                {
                    // Gọi API join phòng
                    var res = await client.PostAsync($"api/arena/join/{roomCode}", null);
                    var json = await res.Content.ReadAsStringAsync();

                    if (!res.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Không thể tham gia: " + json });
                    }

                    return Json(new { success = true, message = "Tham gia phòng thành công!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi: " + ex.Message });
                }
            }
        }

    }
}