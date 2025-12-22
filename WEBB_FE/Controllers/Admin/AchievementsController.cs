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
    public class AchievementsController : Controller
    {
        private readonly string apiBase = "https://localhost:7092/api/admin/thanhtuu";

        // ================================
        // GET: Achievements
        // ================================
        public async Task<ActionResult> Index()
        {
            var list = await GetAchievements();
            return View("~/Views/Admin/Achievements/Index.cshtml", list);
        }

        // ================================
        // CALL API GET
        // ================================
        private async Task<List<ThanhTuuDto>> GetAchievements()
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.GetAsync(apiBase);
                if (!response.IsSuccessStatusCode)
                    return new List<ThanhTuuDto>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ThanhTuuDto>>(json);
            }
        }

        // ================================
        // POST: CREATE
        // ================================
        [HttpPost]
        public async Task<ActionResult> CreateAchievement(ThanhTuuDto model)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiBase, content);

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                    ? "Thêm thành tựu thành công!"
                    : await response.Content.ReadAsStringAsync();

                return RedirectToAction("Index");
            }
        }

        // ================================
        // POST: UPDATE
        // ================================
        [HttpPost]
        public async Task<ActionResult> UpdateAchievement(ThanhTuuDto model)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{apiBase}/{model.DefinitionID}", content);

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                    ? "Cập nhật thành tựu thành công!"
                    : await response.Content.ReadAsStringAsync();

                return RedirectToAction("Index");
            }
        }

        // ================================
        // POST: DELETE
        // ================================
        [HttpPost]
        public async Task<ActionResult> DeleteAchievement(int thanhTuuId)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"{apiBase}/{thanhTuuId}");

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                    ? "Xóa thành tựu thành công!"
                    : await response.Content.ReadAsStringAsync();

                return RedirectToAction("Index");
            }
        }
    }
}
