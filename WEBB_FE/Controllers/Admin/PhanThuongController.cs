using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB_FE.Models.Admin;

namespace WEBB_FE.Controllers.Admin
{
    public class PhanThuongController : Controller
    {
        private readonly string API_URL = "https://localhost:7092/api/admin/phan-thuong";

        // ================================
        // 1. DANH SÁCH LỊCH SỬ PHẦN THƯỞNG
        // ================================
        public async Task<ActionResult> Index(int page = 1)
        {
            var model = new PhanThuongIndexViewModel();

            using (var client = new HttpClient())
            {
                // 🔥 GIỐNG QUIZNGAY – KEY ĐÚNG
                var token = Session["JWT_TOKEN"]?.ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var res = await client.GetAsync(
                    $"{API_URL}/lich-su?page={page}&pageSize=20");

                if (!res.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể tải dữ liệu phần thưởng";
                    return View("~/Views/Admin/PhanThuong/Index.cshtml", model);
                }

                var json = await res.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                model.Total = (int)result.total;
                model.Items = JsonConvert.DeserializeObject<List<PhanThuongDto>>(
                    result.data.ToString()
                );
            }

            return View("~/Views/Admin/PhanThuong/Index.cshtml", model);
        }



        // ================================
        // 2. TẶNG PHẦN THƯỞNG
        // ================================
        [HttpPost]
        public async Task<ActionResult> GiveReward(PhanThuongCreateDto dto)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            using (var client = new HttpClient())
            {
                // 🔥 DÙNG ĐÚNG TOKEN
                var token = Session["JWT_TOKEN"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = await client.PostAsync($"{API_URL}/tang-qua", content);

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Tặng phần thưởng thất bại";
                }
            }

            return RedirectToAction("Index");
        }

        // ================================
        // 3. XÓA PHẦN THƯỞNG
        // ================================
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                // 🔥 DÙNG ĐÚNG TOKEN
                var token = Session["JWT_TOKEN"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var res = await client.DeleteAsync($"{API_URL}/{id}");

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Xóa phần thưởng thất bại";
                }
            }

            return RedirectToAction("Index");
        }
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
