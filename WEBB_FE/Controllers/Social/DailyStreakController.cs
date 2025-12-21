using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WEBB.Models.ViewModels;

namespace WEBB.Controllers
{
    public class DailyStreakController : Controller
    {
        private const string STREAK_SESSION_KEY = "DailyStreak_Data";

        // ===============================
        // VIEW CHÍNH
        // ===============================
        public ActionResult Index()
        {
            var userId = GetUserId();
            var model = GetDailyStreak(userId);

            model.IsLoggedIn = User.Identity.IsAuthenticated;
            model.UserName = model.IsLoggedIn ? User.Identity.Name : "Khách";

            return View(model);
        }

        // ===============================
        // NHẬN THƯỞNG (AJAX)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NhanThuong()
        {
            var userId = GetUserId();
            var model = ClaimReward(userId);

            return Json(new
            {
                success = true,
                message = model.Message,
                soNgayLienTiep = model.SoNgayLienTiep,
                diemThuong = model.DiemThuong,
                tongDiem = model.TongDiemDaNhan,
                daNhanThuongHomNay = model.DaNhanThuongHomNay,
                coTheNhanThuong = model.CoTheNhanThuong
            });
        }

        // ===============================
        // STATUS (OPTIONAL)
        // ===============================
        [HttpGet]
        public ActionResult GetStreakStatus()
        {
            var model = GetDailyStreak(GetUserId());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        // ===============================
        // CORE LOGIC
        // ===============================
        private DailyStreakViewModel GetDailyStreak(string userId)
        {
            var streak = Session[$"{STREAK_SESSION_KEY}_{userId}"] as DailyStreakViewModel;

            if (streak == null)
            {
                streak = new DailyStreakViewModel
                {
                    SoNgayLienTiep = 1,
                    NgayCapNhatCuoi = DateTime.Today,
                    CoTheNhanThuong = true
                };

                SaveToSession(userId, streak);
            }
            else
            {
                ProcessStreakLogic(userId, streak);
            }

            return streak;
        }

        private DailyStreakViewModel ClaimReward(string userId)
        {
            var streak = GetDailyStreak(userId);

            if (streak.DaNhanThuongHomNay)
            {
                streak.Message = "Bạn đã nhận thưởng hôm nay rồi!";
                return streak;
            }

            streak.DaNhanThuongHomNay = true;
            streak.CoTheNhanThuong = false;
            streak.NgayCapNhatCuoi = DateTime.Today;
            streak.TongDiemDaNhan += streak.DiemThuong;

            if (!streak.LichSu7Ngay.Any(d => d.Date == DateTime.Today))
            {
                streak.LichSu7Ngay.Add(DateTime.Today);
                streak.LichSu7Ngay = streak.LichSu7Ngay
                    .OrderByDescending(d => d)
                    .Take(7)
                    .ToList();
            }

            streak.Message = $"🎉 Bạn nhận được {streak.DiemThuong} điểm thưởng!";

            SaveToSession(userId, streak);
            return streak;
        }

        private void ProcessStreakLogic(string userId, DailyStreakViewModel streak)
        {
            var today = DateTime.Today;

            if (streak.NgayCapNhatCuoi.HasValue)
            {
                int diff = (today - streak.NgayCapNhatCuoi.Value.Date).Days;

                if (diff == 1)
                {
                    streak.SoNgayLienTiep++;
                    streak.CoTheNhanThuong = true;
                    streak.DaNhanThuongHomNay = false;
                    streak.NgayCapNhatCuoi = today;
                }
                else if (diff > 1)
                {
                    streak.SoNgayLienTiep = 1;
                    streak.CoTheNhanThuong = true;
                    streak.DaNhanThuongHomNay = false;
                    streak.NgayCapNhatCuoi = today;
                }
                else
                {
                    streak.CoTheNhanThuong = !streak.DaNhanThuongHomNay;
                }
            }

            CalculateBonus(streak);
            SaveToSession(userId, streak);
        }

        private void CalculateBonus(DailyStreakViewModel streak)
        {
            if (streak.SoNgayLienTiep >= 30)
            {
                streak.DiemThuong = 30;
                streak.BonusMultiplier = 3;
            }
            else if (streak.SoNgayLienTiep >= 7)
            {
                streak.DiemThuong = 20;
                streak.BonusMultiplier = 2;
            }
            else
            {
                streak.DiemThuong = 10;
                streak.BonusMultiplier = 1;
            }
        }

        private void SaveToSession(string userId, DailyStreakViewModel streak)
        {
            Session[$"{STREAK_SESSION_KEY}_{userId}"] = streak;
        }

        private string GetUserId()
        {
            if (User.Identity.IsAuthenticated)
                return User.Identity.Name;

            return Request.UserHostAddress;
        }
    }
}
