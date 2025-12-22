using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Quiz;

namespace WEBB.Controllers.Admin
{
    public class DifficultiesController : Controller
    {
        private readonly string apiBase = "https://localhost:7092/api/admin/dokho/";

        // ================================
        // GET: Difficulties
        // ================================
        public async Task<ActionResult> Index()
        {
            var list = await GetDifficulties();
            return View("~/Views/Admin/Difficulties/Index.cshtml", list);
        }

        private async Task<List<DoKhoDto>> GetDifficulties()
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
                    return new List<DoKhoDto>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<DoKhoDto>>(json);
            }
        }

        // ================================
        // POST: CREATE
        // ================================
        [HttpPost]
        public async Task<ActionResult> Create(DoKhoDto model)
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
                    ? "Thêm độ khó thành công!"
                    : await response.Content.ReadAsStringAsync();

                return RedirectToAction("Index");
            }
        }

        // ================================
        // GET: EDIT
        // ================================
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(apiBase + id);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy độ khó.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<DoKhoDto>(json);

                return View("~/Views/Admin/Difficulties/Edit.cshtml", dto);
            }
        }

        // ================================
        // POST: EDIT
        // ================================
        [HttpPost]
        public async Task<ActionResult> Edit(DoKhoDto model)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiBase + model.DoKhoID, content);

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                    ? "Cập nhật độ khó thành công!"
                    : await response.Content.ReadAsStringAsync();

                return RedirectToAction("Index");
            }
        }

        // ================================
        // POST: DELETE
        // ================================
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync(apiBase + id);

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                    ? "Xóa độ khó thành công!"
                    : await response.Content.ReadAsStringAsync();

                return RedirectToAction("Index");
            }
        }
    }
}
