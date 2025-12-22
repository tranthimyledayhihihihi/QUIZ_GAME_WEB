using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.ViewModels;

public class DailyStreakController : Controller
{
    private readonly string API_BASE = "https://localhost:7092/api/user/achievement";

    // =========================
    // GET: /DailyStreak
    // =========================
    public async Task<ActionResult> Index()
    {
        var model = new DailyStreakViewModel();

        // 1️⃣ Check login
        if (!User.Identity.IsAuthenticated)
        {
            model.IsLoggedIn = false;
            model.Message = "Vui lòng đăng nhập";
            return View(model);
        }

        model.IsLoggedIn = true;
        model.UserName = User.Identity.Name;

        var token = Session["JWT_TOKEN"]?.ToString();
        if (string.IsNullOrEmpty(token))
        {
            model.Message = "Phiên đăng nhập không hợp lệ";
            return View(model);
        }

        // 2️⃣ Gọi API lấy streak
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync($"{API_BASE}/streak");

            if (!res.IsSuccessStatusCode)
            {
                model.Message = "Không thể tải chuỗi ngày";
                return View("~/Views/User/Menu/DailyStreak.cshtml", model);
            }

            var json = await res.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(json);

            model.SoNgayLienTiep = (int)data.soNgayLienTiep;

            // ✅ PHẢI GÁN NGÀY TRƯỚC
            model.NgayCapNhatCuoi = data.ngayCapNhatCuoi != null
                ? (DateTime)data.ngayCapNhatCuoi
                : (DateTime?)null;

            // ✅ BUILD TIMELINE 7 NGÀY TỪ DB
            model.LichSu7Ngay = new List<DateTime>();

            if (model.NgayCapNhatCuoi.HasValue && model.SoNgayLienTiep > 0)
            {
                for (int i = 0; i < model.SoNgayLienTiep; i++)
                {
                    model.LichSu7Ngay.Add(
                        model.NgayCapNhatCuoi.Value.Date.AddDays(-i)
                    );
                }
            }

            model.Message = "Nhận thưởng ngày để tiếp tục chuỗi!";
        }

        return View("~/Views/User/Menu/DailyStreak.cshtml", model);
    }

    // =========================
    // POST: /DailyStreak/NhanThuong
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> NhanThuong()
    {
        var token = Session["JWT_TOKEN"]?.ToString();
        if (string.IsNullOrEmpty(token))
            return Json(new { success = false, message = "Chưa đăng nhập" });
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API backend
            var res = await client.PostAsync($"{API_BASE}/daily-reward", null);
            var json = await res.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);
            // QUAN TRỌNG: Lấy đúng trạng thái awarded từ Backend
            bool success = result.awarded ?? false;
            string msg = result.message ?? "Lỗi cập nhật";
            return Json(new { success = success, message = msg });
        }
    }
}