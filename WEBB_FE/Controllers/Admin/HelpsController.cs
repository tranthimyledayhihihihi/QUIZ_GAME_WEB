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
    public class HelpsController : Controller
    {
        private readonly string api =
            "https://localhost:7092/api/admin/he-thong/tro-giup";

        // ===============================
        // GET: /Admin/Help
        // ===============================

        public async Task<ActionResult> Index()
        {
            var list = new List<TroGiupDto>();

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(api);
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể tải danh sách trợ giúp";
                    return View("~/Views/Admin/Helps/Index.cshtml", list);
                }

                var json = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<TroGiupDto>>(json);
            }

            return View("~/Views/Admin/Helps/Index.cshtml", list);
        }

        // ===============================
        // POST: CREATE
        // ===============================
        // ===============================
        // GET: /Admin/Help/Create
        // ===============================
        

        [HttpPost]
        public async Task<ActionResult> Create(TroGiupDto model)
        {
            using (var client = CreateClient())
            {
                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(api, content);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể thêm trợ giúp";
                    return RedirectToAction("Create");
                }
            }

            return RedirectToAction("Index");
        }


        // ===============================
        // POST: DELETE
        // ===============================
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = CreateClient())
            {
                await client.DeleteAsync($"{api}/{id}");
            }

            return RedirectToAction("Index");
        }

        // ===============================
        // HELPER
        // ===============================
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
