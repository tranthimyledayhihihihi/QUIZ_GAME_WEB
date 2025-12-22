using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Admin;

namespace WEBB.Controllers.Admin
{
    public class ProfileController : Controller
    {
        private readonly string API_URL = "https://localhost:7092/api/user/profile/me";

        // =========================
        // GET: /Admin/Profile
        // =========================
        public async Task<ActionResult> Index()
        {
            ProfileDto model = null;

            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var res = await client.GetAsync(API_URL);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProfileDto>(json);
                }
                else
                {
                    TempData["Error"] = "Không tải được thông tin hồ sơ.";
                }
            }

            return View("~/Views/Admin/Profile/Index.cshtml", model);
        }

        // =========================
        // POST: UPDATE PROFILE
        // =========================
        [HttpPost]
        public async Task<ActionResult> Update(ProfileDto model)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var payload = JsonConvert.SerializeObject(new
                {
                    tenDangNhap = model.TenDangNhap,
                    email = model.Email,
                    hoTen = model.HoTen
                });

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var res = await client.PutAsync(API_URL, content);

                TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                    res.IsSuccessStatusCode
                        ? "Cập nhật hồ sơ thành công!"
                        : await res.Content.ReadAsStringAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
