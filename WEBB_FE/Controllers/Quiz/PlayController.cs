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
        // 👇 ĐỔI PORT NÀY CHO ĐÚNG VỚI BACKEND CỦA BẠN
        private readonly string _apiBaseUrl = "https://localhost:7092/api/choi";

        // GET: Quiz/Play
        public ActionResult Index()
        {
            return View("~/Views/Quiz/Play/Index.cshtml");
        }

        // POST: Quiz/Play/StartGame
        [HttpPost]
        public async Task<ActionResult> StartGame(int ChuDeID, int DoKhoID, int SoLuongCauHoi)
        {
            try
            {
                // 👇 THÊM DÒNG NÀY: Bỏ qua kiểm tra SSL (Chỉ dùng cho Dev/Localhost)
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient())
                {
                    var token = GetToken();
                    if (!string.IsNullOrEmpty(token))
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var payload = new
                    {
                        ChuDeID = ChuDeID,
                        DoKhoID = DoKhoID,
                        SoLuongCauHoi = SoLuongCauHoi
                    };

                    var jsonContent = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{_apiBaseUrl}/start", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return Content(responseString, "application/json");
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new HttpStatusCodeResult(response.StatusCode, "Lỗi Backend: " + errorContent);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Lỗi Server Web: " + ex.Message);
            }
        }

        // POST: Quiz/Play/SubmitAnswer
        [HttpPost]
        public async Task<ActionResult> SubmitAnswer(int QuizAttemptID, int CauHoiID, string DapAnDaChon)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;
                int userId = 2; // Giả định UserID

                using (var client = new HttpClient())
                {
                    var token = GetToken();
                    if (!string.IsNullOrEmpty(token))
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var payload = new
                    {
                        QuizAttemptID = QuizAttemptID,
                        CauHoiID = CauHoiID,
                        DapAnDaChon = DapAnDaChon,
                        UserID = userId
                    };

                    var jsonContent = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{_apiBaseUrl}/submit", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return Content(responseString, "application/json");
                    }

                    return new HttpStatusCodeResult(response.StatusCode, "Lỗi nộp bài");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Lỗi Server Web: " + ex.Message);
            }
        }

        // POST: Quiz/Play/NextQuestion
        [HttpPost]
        public async Task<ActionResult> NextQuestion(int attemptId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Bỏ qua SSL (nếu cần)
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;

                    var token = GetToken();
                    if (!string.IsNullOrEmpty(token))
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    // Gọi API lấy câu tiếp theo
                    var response = await client.GetAsync($"{_apiBaseUrl}/next/{attemptId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return Content(responseString, "application/json");
                    }

                    // Nếu Backend trả về 404 Not Found -> Tức là đã hết câu hỏi
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Json(new { isFinished = true });
                    }

                    return new HttpStatusCodeResult(response.StatusCode, "Lỗi lấy câu hỏi");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Lỗi Server: " + ex.Message);
            }
        }

        private string GetToken()
        {
            // 👇 Dán Token thật của bạn vào đây
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIyIiwidW5pcXVlX25hbWUiOiJwbGF5ZXIwMSIsInJvbGUiOiJQbGF5ZXIiLCJuYmYiOjE3NjQ2ODc4ODgsImV4cCI6MTc2NDY5NTA4OCwiaWF0IjoxNzY0Njg3ODg4LCJpc3MiOiJodHRwczovL3F1aXpnYW1lLmNvbSIsImF1ZCI6Imh0dHBzOi8vcXVpemdhbWUuY29tIn0.WkIjaW--GZmhHPGoigoeN6rdevUXehdqTI0uXzbV4ok";
        }
    }
}