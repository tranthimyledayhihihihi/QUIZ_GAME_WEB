using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using WEBB.Models.Quiz;

namespace WEBB.Controllers.Quiz
{
    public class PlayController : Controller
    {
        // Backend API URL
        private readonly string _apiBaseUrl = "https://localhost:7092/api/choi";

        // ================================
        // GET: /Quiz/Play?roomId=123
        // ================================
        public ActionResult Index(int? roomId)
        {
            // Đưa roomId vào ViewBag thay vì để View tự gọi QueryString
            ViewBag.RoomId = roomId;
            return View("~/Views/Quiz/Play/Index.cshtml");
        }

        // ================================
        // POST: StartGame
        // ================================
        [HttpPost]
        public async Task<ActionResult> StartGame(int ChuDeID, int DoKhoID, int SoLuongCauHoi)
        {
            try
            {
                // Bỏ qua SSL cho localhost
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    (s, cert, chain, sslErr) => true;

                var payload = new
                {
                    ChuDeID,
                    DoKhoID,
                    SoLuongCauHoi
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (var client = CreateHttpClient())
                {
                    var response = await client.PostAsync($"{_apiBaseUrl}/start", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var err = await response.Content.ReadAsStringAsync();
                        Response.StatusCode = (int)response.StatusCode;
                        // Trả về lỗi dưới dạng JSON Body để client đọc được, thay vì nhét vào Header gây lỗi Protocol
                        return Content(err, "application/json");
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    return Content(result, "application/json");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Lỗi Web Server: " + ex.Message);
            }
        }

        // Hàm giải mã Token để lấy UserID
        private int GetUserIdFromToken()
        {
            try
            {
                var token = GetToken();
                if (string.IsNullOrEmpty(token)) return 0;

                var parts = token.Split('.');
                if (parts.Length < 2) return 0;

                var payload = parts[1];
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                
                var jsonBytes = Convert.FromBase64String(payload.Replace("-", "+").Replace("_", "/"));
                var jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                // Dùng JObject thay vì dynamic để access key có ký tự đặc biệt
                var json = Newtonsoft.Json.Linq.JObject.Parse(jsonString);

                // 1. Thử các key ngắn gọn
                if (json["UserID"] != null) return (int)json["UserID"];
                if (json["userid"] != null) return (int)json["userid"];
                if (json["nameid"] != null) return (int)json["nameid"];
                
                // 2. Thử key dài chuẩn SOAP/XML
                // ClaimTypes.NameIdentifier thường ra cái này nếu không cấu hình mapping
                string longKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
                if (json[longKey] != null) return (int)json[longKey];

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        // ================================
        // POST: SubmitAnswer
        // ================================
        [HttpPost]
        public async Task<ActionResult> SubmitAnswer(int QuizAttemptID, int CauHoiID, string DapAnDaChon)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    (s, cert, chain, sslErr) => true;

                // Lấy UserID thật từ Token
                int userId = GetUserIdFromToken();

                var payload = new
                {
                    QuizAttemptID,
                    CauHoiID,
                    DapAnDaChon,
                    UserID = userId // Gửi ID chính chủ
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (var client = CreateHttpClient())
                {
                    var response = await client.PostAsync($"{_apiBaseUrl}/submit", content);

                    if (!response.IsSuccessStatusCode)
                        return new HttpStatusCodeResult(response.StatusCode, "Lỗi nộp bài");

                    var result = await response.Content.ReadAsStringAsync();
                    return Content(result, "application/json");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Lỗi Web Server: " + ex.Message);
            }
        }

        // ================================
        // POST: NextQuestion
        // ================================
        [HttpPost]
        public async Task<ActionResult> NextQuestion(int attemptId)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/next/{attemptId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return Content(result, "application/json");
                    }

                    // 404 = hết câu hỏi
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return Json(new { isFinished = true });

                    return new HttpStatusCodeResult(response.StatusCode, "Lỗi lấy câu hỏi");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Lỗi Server: " + ex.Message);
            }
        }
        // POST: EndGame (BẮT BUỘC PHẢI CÓ ĐỂ LƯU LỊCH SỬ)
        // ================================
        [HttpPost]
        public async Task<ActionResult> EndGame(int attemptId)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    // Gọi Backend để chốt kết quả và tính điểm
                    var response = await client.PostAsync($"{_apiBaseUrl}/end/{attemptId}", null);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return Content(result, "application/json");
                    }

                    return new HttpStatusCodeResult(response.StatusCode, "Lỗi kết thúc game");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Lỗi Server: " + ex.Message);
            }
        }

        // ================================
        // PRIVATE: Tạo HttpClient chuẩn
        // ================================
        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            // Bỏ qua SSL Localhost
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (s, cert, chain, sslErr) => true;

            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // ================================
        // JWT Token
        // ================================
        private string GetToken()
        {
            return Session["JWT_TOKEN"] as string;
        }
    }
}