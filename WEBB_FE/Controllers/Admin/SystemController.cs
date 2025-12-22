using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.System;

namespace WEBB.Controllers.Admin
{
    public class SystemController : Controller
    {
        private readonly string api =
            "https://localhost:7092/api/admin/he-thong/settings";

        // ===============================
        // GET: /Admin/System
        // ===============================
        public async Task<ActionResult> Index()
        {
            var list = new List<SystemSettingDto>();

            using (var client = CreateClient())
            {
                var response = await client.GetAsync(api);
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể tải cấu hình hệ thống";
                    return View("~/Views/Admin/System/Index.cshtml", list);
                }

                var json = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<SystemSettingDto>>(json);
            }

            return View("~/Views/Admin/System/Index.cshtml", list);
        }

        // ===============================
        // POST: CREATE
        // ===============================
        [HttpPost]
        public async Task<ActionResult> Create(SystemSettingDto model)
        {
            using (var client = CreateClient())
            {
                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(api, content);
                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                        ? "Thêm cấu hình thành công"
                        : await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction("Index");
        }

        // ===============================
        // POST: UPDATE
        // ===============================
        [HttpPost]
        public async Task<ActionResult> Edit(SystemSettingDto model)
        {
            using (var client = CreateClient())
            {
                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{api}/{model.Key}", content);
                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                        ? "Cập nhật cấu hình thành công"
                        : await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction("Index");
        }

        // ===============================
        // HELPER: HttpClient + JWT
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
