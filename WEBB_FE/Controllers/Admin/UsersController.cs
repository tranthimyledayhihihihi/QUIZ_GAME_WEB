using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class UsersController : Controller
    {
        private readonly string apiBase = "https://localhost:7092/api/admin/nguoidung";

        // ==================================================
        // INDEX + SEARCH (UserID hoặc keyword)
        // ==================================================
        public async Task<ActionResult> Index(int page = 1, int pageSize = 20, string keyword = null)
        {
            // 🔎 Nếu keyword là số → tìm theo UserID
            if (!string.IsNullOrEmpty(keyword) && int.TryParse(keyword, out int userId))
            {
                var user = await GetUserById(userId);

                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy người dùng!";
                    return View("~/Views/Admin/Users/Index.cshtml",
                        new List<AdminUserDto>());
                }

                return View("~/Views/Admin/Users/Index.cshtml",
                    new List<AdminUserDto> { user });
            }

            // 🔎 Tìm theo keyword (username/email)
            var list = await GetUsers(page, pageSize, keyword);

            if (list == null || list.Count == 0)
                TempData["Error"] = "Không có dữ liệu người dùng!";

            return View("~/Views/Admin/Users/Index.cshtml", list);
        }

        // ==================================================
        // GET LIST
        // ==================================================
        private async Task<List<AdminUserDto>> GetUsers(int page, int pageSize, string keyword)
        {
            using (var client = CreateClient())
            {
                var url = $"{apiBase}?page={page}&pageSize={pageSize}&keyword={keyword}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<AdminUserDto>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<AdminUserDto>>(json)
                       ?? new List<AdminUserDto>();
            }
        }

        // ==================================================
        // GET BY ID
        // ==================================================
        private async Task<AdminUserDto> GetUserById(int id)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetAsync($"{apiBase}/{id}");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AdminUserDto>(json);
            }
        }

        // ==================================================
        // DETAILS
        // ==================================================
        public async Task<ActionResult> Details(int id)
        {
            var user = await GetUserById(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng!";
                return RedirectToAction("Index");
            }

            return View("~/Views/Admin/Users/Details.cshtml", user);
        }

        // ==================================================
        // LOCK (VIEW)
        // ==================================================
        public async Task<ActionResult> Lock(int id)
        {
            var user = await GetUserById(id);

            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại!";
                return RedirectToAction("Index");
            }

            if (user.RoleName == "SuperAdmin")
            {
                TempData["Error"] = "Không thể khóa tài khoản SuperAdmin!";
                return RedirectToAction("Index");
            }

            ViewBag.UserId = id;
            return View("~/Views/Admin/Users/Lock.cshtml");
        }

        // ==================================================
        // LOCK (POST)
        // ==================================================
        [HttpPost]
        public async Task<ActionResult> LockConfirm(int id)
        {
            var user = await GetUserById(id);

            if (user.RoleName == "SuperAdmin")
            {
                TempData["Error"] = "Không thể khóa tài khoản SuperAdmin!";
                return RedirectToAction("Index");
            }

            using (var client = CreateClient())
            {
                await client.PostAsync($"{apiBase}/khoa/{id}", null);
                TempData["Success"] = "Khóa tài khoản thành công!";
            }

            return RedirectToAction("Index");
        }

        // ==================================================
        // UNLOCK (VIEW)
        // ==================================================
        public ActionResult Unlock(int id)
        {
            ViewBag.UserId = id;
            return View("~/Views/Admin/Users/Unlock.cshtml");
        }

        // ==================================================
        // UNLOCK (POST)
        // ==================================================
        [HttpPost]
        public async Task<ActionResult> UnlockConfirm(int id)
        {
            using (var client = CreateClient())
            {
                await client.PostAsync($"{apiBase}/mo-khoa/{id}", null);
                TempData["Success"] = "Mở khóa tài khoản thành công!";
            }

            return RedirectToAction("Index");
        }

        // ==================================================
        // UPDATE ROLE (VIEW)
        // ==================================================
        public ActionResult UpdateRole(int id)
        {
            ViewBag.UserId = id;
            return View("~/Views/Admin/Users/UpdateRole.cshtml");
        }

        // ==================================================
        // UPDATE ROLE (POST) – CHUẨN
        // ==================================================
        [HttpPost]
        public async Task<ActionResult> UpdateRoleConfirm(int userId, int newRoleId)
        {
            var user = await GetUserById(userId);

            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại!";
                return RedirectToAction("Index");
            }

            // ❌ Không cho tự đổi quyền chính mình
            if (Session["USER_ID"] != null &&
                user.UserID == (int)Session["USER_ID"])
            {
                TempData["Error"] = "Bạn không thể tự thay đổi quyền của chính mình!";
                return RedirectToAction("Index");
            }

            using (var client = CreateClient())
            {
                var response = await client.PostAsync(
                    $"{apiBase}/phan-quyen/{userId}/{newRoleId}", null);

                if (!response.IsSuccessStatusCode)
                    TempData["Error"] = "Cập nhật vai trò thất bại!";
                else
                    TempData["Success"] = "Cập nhật vai trò thành công!";
            }

            return RedirectToAction("Index");
        }

        // ==================================================
        // HELPER: CREATE CLIENT WITH JWT
        // ==================================================
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var token = Session["JWT_TOKEN"]?.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}
