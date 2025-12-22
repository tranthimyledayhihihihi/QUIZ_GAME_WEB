using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Quiz;

namespace WEBB.Controllers.Admin
{
    public class TopicsController : Controller
    {
        private readonly string apiBase = "https://localhost:7092/api/admin/QLChuDe/";

        // ============================================
        // GET: Admin/Topics
        // ============================================
        public async Task<ActionResult> Index()
        {
            var list = await GetTopics();
            return View("~/Views/Admin/Topics/Index.cshtml", list);
        }

        // ============================================
        // GET LIST FROM API
        // ============================================
        private async Task<List<ChuDeDto>> GetTopics()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiBase);

                if (!response.IsSuccessStatusCode)
                    return new List<ChuDeDto>();

                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ChuDeDto>>(json);
            }
        }

        // ============================================
        // GET: Admin/Topics/Edit/{id}
        // (Hiển thị form chỉnh sửa)
        // ============================================
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiBase + id);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy chủ đề.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<ChuDeDto>(json);

                return View("~/Views/Admin/Topics/Edit.cshtml", dto);
            }
        }

        // ============================================
        // POST: CREATE NEW TOPIC
        // ============================================
        [HttpPost]
        public async Task<ActionResult> Create(ChuDeDto model)
        {
            using (var client = new HttpClient())
            {
                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiBase, content);

                if (!response.IsSuccessStatusCode)
                    TempData["Error"] = await response.Content.ReadAsStringAsync();
                else
                    TempData["Success"] = "Thêm chủ đề thành công!";  // Thêm thông báo thành công

                return RedirectToAction("Index");  // Quay lại trang danh sách chủ đề
            }
        }



        // ============================================
        // POST: EDIT TOPIC
        // ============================================
        [HttpPost]
        public async Task<ActionResult> Edit(ChuDeDto model)
        {
            // Kiểm tra và xử lý trạng thái
            model.TrangThai = Request["TrangThai"] == "true" || Request["TrangThai"] == "True";

            using (var client = new HttpClient())
            {
                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiBase + model.ChuDeID, content);

                if (!response.IsSuccessStatusCode)
                    TempData["Error"] = await response.Content.ReadAsStringAsync();
                else
                    TempData["Success"] = "Cập nhật chủ đề thành công!";  // Thêm thông báo thành công

                return RedirectToAction("Index");  // Quay lại trang danh sách chủ đề
            }
        }




        // ============================================
        // POST: DELETE TOPIC
        // ============================================
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.DeleteAsync(apiBase + id);

                if (!response.IsSuccessStatusCode)
                    TempData["Error"] = await response.Content.ReadAsStringAsync();
                else
                    TempData["Success"] = "Xóa chủ đề thành công!";  // Thêm thông báo thành công

                return RedirectToAction("Index");  // Quay lại trang danh sách chủ đề
            }
        }


    }
}
