using System;
using System.Collections.Generic;
using System.Configuration; // Để đọc Web.config nếu cần
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WEBB_FE.Models.ViewModels;
namespace WEBB_FE.Controllers.User
{
    public class UserProfileController : Controller
    {
        private readonly string _apiBaseUrl = "https://localhost:7092/";
        public async Task<ActionResult> Index()
        {
            // 1. Lấy Token từ Session (Kiểu MVC cũ)
            var token = Session["JWT_TOKEN"] as string;
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new UserProfileViewModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                try
                {
                    // 2. Gọi API lấy thông tin cá nhân
                    // Lưu ý: Đường dẫn API cần check lại chính xác trong Backend
                    var resInfo = await client.GetAsync("api/user/profile/me");
                    if (resInfo.IsSuccessStatusCode)
                    {
                        var dataInfo = await resInfo.Content.ReadAsStringAsync();
                        dynamic infoObj = JsonConvert.DeserializeObject(dataInfo);
                        model.UserID = infoObj.userID ?? 0;
                        model.TenDangNhap = infoObj.tenDangNhap;
                        model.Email = infoObj.email;
                        model.HoTen = infoObj.hoTen;
                        string avatar = infoObj.anhDaiDien;
                        if (string.IsNullOrEmpty(avatar))
                        {
                            // Dùng avatar tự tạo theo tên
                            var name = model.HoTen ?? "User";
                            avatar = $"https://ui-avatars.com/api/?name={name}&background=random&color=fff";
                        }
                        model.AnhDaiDien = avatar;
                        model.VaiTro = infoObj.vaiTro;
                    }
                    // 3. Gọi API lấy Streak
                    var resStreak = await client.GetAsync("api/LichSuChoi/streak");
                    if (resStreak.IsSuccessStatusCode)
                    {
                        var dataStreak = await resStreak.Content.ReadAsStringAsync();
                        dynamic streakObj = JsonConvert.DeserializeObject(dataStreak);
                        model.StreakCount = streakObj.soNgayLienTiep ?? 0;
                    }
                    // 4. Gọi API lấy Lịch sử đấu
                    var resHistory = await client.GetAsync("api/LichSuChoi/my?pageNumber=1&pageSize=5");
                    if (resHistory.IsSuccessStatusCode)
                    {
                        var dataHistory = await resHistory.Content.ReadAsStringAsync();
                        dynamic histObj = JsonConvert.DeserializeObject(dataHistory);
                        model.TongTranDaChoi = histObj.tongSoKetQua ?? 0;
                        if (histObj.danhSach != null)
                        {
                            foreach (var item in histObj.danhSach)
                            {
                                model.LichSuDau.Add(new MatchHistoryItem
                                {
                                    QuizAttemptID = item.quizAttemptID,
                                    TenQuiz = "Quiz",
                                    Diem = item.diem,
                                    SoCauDung = item.soCauDung,
                                    NgayChoi = item.ngayBatDau
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi kết nối: " + ex.Message;
                }
            }
            return View("~/Views/User/Profile/Index.cshtml", model);
        }
    }
}