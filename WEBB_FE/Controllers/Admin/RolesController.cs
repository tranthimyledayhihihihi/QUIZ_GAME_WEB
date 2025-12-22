using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class RolesController : Controller
    {
        private readonly string roleApi = "https://localhost:7092/api/admin/vaitro";
        private readonly string permissionApi = "https://localhost:7092/api/admin/quyen";

        // =========================
        // GET: /Admin/Roles
        // =========================
        public async Task<ActionResult> Index()
        {
            var list = new List<RoleDto>();

            using (var client = CreateClient())
            {
                var res = await client.GetAsync(roleApi);
                if (res.IsSuccessStatusCode)
                {
                    list = JsonConvert.DeserializeObject<List<RoleDto>>(
                        await res.Content.ReadAsStringAsync());
                }
                else
                {
                    ViewBag.Error = "Không thể tải danh sách vai trò";
                }
            }

            return View("~/Views/Admin/Roles/Index.cshtml", list);
        }

        // =========================
        // POST: Tạo vai trò
        // =========================
        [HttpPost]
        public async Task<ActionResult> Create(string tenVaiTro)
        {
            using (var client = CreateClient())
            {
                var payload = JsonConvert.SerializeObject(tenVaiTro);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var res = await client.PostAsync(roleApi, content);
                if (!res.IsSuccessStatusCode)
                    TempData["Error"] = "Không thể tạo vai trò";
                else
                    TempData["Success"] = "Tạo vai trò thành công";
            }

            return RedirectToAction("Index");
        }

        // =========================
        // GET: Chi tiết vai trò
        // =========================
        public async Task<ActionResult> Detail(int id)
        {
            RoleDetailDto model = null;

            using (var client = CreateClient())
            {
                var res = await client.GetAsync(roleApi + "/" + id);
                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể tải chi tiết vai trò";
                    return RedirectToAction("Index");
                }

                model = JsonConvert.DeserializeObject<RoleDetailDto>(
                    await res.Content.ReadAsStringAsync());
            }

            return View("~/Views/Admin/Roles/Detail.cshtml", model);
        }

        // =========================
        // GET: Gán quyền
        // =========================
        public async Task<ActionResult> AssignPermissions(int id)
        {
            RoleDetailDto role = null;
            List<PermissionDto> allPermissions = new List<PermissionDto>();

            using (var client = CreateClient())
            {
                var roleRes = await client.GetAsync(roleApi + "/" + id);
                role = JsonConvert.DeserializeObject<RoleDetailDto>(
                    await roleRes.Content.ReadAsStringAsync());

                var permRes = await client.GetAsync(permissionApi);
                if (permRes.IsSuccessStatusCode)
                {
                    allPermissions = JsonConvert.DeserializeObject<List<PermissionDto>>(
                        await permRes.Content.ReadAsStringAsync());
                }
            }

            ViewBag.AllPermissions = allPermissions;
            return View("~/Views/Admin/Roles/AssignPermissions.cshtml", role);
        }

        // =========================
        // POST: Lưu gán quyền
        // =========================
        [HttpPost]
        public async Task<ActionResult> AssignPermissions(int roleId, int[] quyenIds)
        {
            using (var client = CreateClient())
            {
                var payload = JsonConvert.SerializeObject(quyenIds ?? new int[0]);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var res = await client.PostAsync(
                    roleApi + "/" + roleId + "/gan-quyen", content);

                if (!res.IsSuccessStatusCode)
                    TempData["Error"] = "Không thể cập nhật quyền";
                else
                    TempData["Success"] = "Cập nhật quyền thành công";
            }

            return RedirectToAction("Detail", new { id = roleId });
        }

        // =========================
        // POST: Xóa vai trò
        // =========================
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = CreateClient())
            {
                await client.DeleteAsync(roleApi + "/" + id);
            }
            return RedirectToAction("Index");
        }

        // =========================
        // HTTP CLIENT + JWT
        // =========================
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var token = Session["JWT_TOKEN"] != null ? Session["JWT_TOKEN"].ToString() : null;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }
    }
}
