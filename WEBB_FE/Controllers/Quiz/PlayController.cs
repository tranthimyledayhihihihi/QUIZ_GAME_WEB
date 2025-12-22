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
            return View("~/Views/User/Quiz/Play/Index.cshtml");
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

                int userId = 2; // TODO: Lấy UserID thật từ Session

                var payload = new
                {
                    QuizAttemptID,
                    CauHoiID,
                    DapAnDaChon,
                    UserID = userId
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
