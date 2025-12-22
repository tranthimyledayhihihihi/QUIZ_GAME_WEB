using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBB.Models.Quiz;

namespace WEBB.Controllers.Admin
{
    public class QuestionsController : Controller
    {
        private readonly string apiBase = "https://localhost:7092/api/admin/QLCauHoi";
        private readonly string chuDeApi = "https://localhost:7092/api/admin/QLChuDe";
        private readonly string doKhoApi = "https://localhost:7092/api/admin/dokho";

        // ================================
        // GET: Questions
        // ================================
        public async Task<ActionResult> Index()
        {
            await LoadDropdownData(); // 🔥 QUAN TRỌNG
            var list = await GetQuestions();
            return View("~/Views/Admin/Questions/Index.cshtml", list);
        }


        // ================================
        // CALL API GET
        // ================================
        private async Task<List<CauHoiDto>> GetQuestions()
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
                    return new List<CauHoiDto>();

                var json = await response.Content.ReadAsStringAsync();

                // API trả về { total, page, pageSize, data }
                dynamic obj = JsonConvert.DeserializeObject(json);
                return JsonConvert.DeserializeObject<List<CauHoiDto>>(obj.data.ToString());
            }
        }
        private async Task LoadDropdownData()
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                // Chủ đề
                var chuDeRes = await client.GetAsync(chuDeApi);
                if (chuDeRes.IsSuccessStatusCode)
                {
                    var json = await chuDeRes.Content.ReadAsStringAsync();
                    ViewBag.ChuDes = JsonConvert.DeserializeObject<List<ChuDeDto>>(json);
                }
                else
                {
                    ViewBag.ChuDes = new List<ChuDeDto>();
                }

                // Độ khó
                var doKhoRes = await client.GetAsync(doKhoApi);
                if (doKhoRes.IsSuccessStatusCode)
                {
                    var json = await doKhoRes.Content.ReadAsStringAsync();
                    ViewBag.DoKhos = JsonConvert.DeserializeObject<List<DoKhoDto>>(json);
                }
                else
                {
                    ViewBag.DoKhos = new List<DoKhoDto>();
                }
            }
        }

        // ================================
        // GET: CREATE
        // ================================
        [HttpGet]
        public ActionResult Create()
        {
            return View("~/Views/Admin/Questions/Create.cshtml");
        }

        // ================================
        // POST: CREATE
        // ================================
        [HttpPost]
        public async Task<ActionResult> Create(CauHoiDto model)
        {
            model.NgayTao = DateTime.UtcNow;
            model.TrangThaiDuyet = "CHO_DUYET";
            model.AdminDuyetID = 0;
            model.QuizTuyChinhID = 0;

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
                        ? "Thêm câu hỏi thành công!"
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

                var response = await client.GetAsync($"{apiBase}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy câu hỏi.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<CauHoiDto>(json);

                return View("~/Views/Admin/Questions/Edit.cshtml", dto);
            }
        }


        // ================================
        // POST: EDIT
        // ================================
        [HttpPost]
        public async Task<ActionResult> Edit(CauHoiDto model)
        {
            using (var client = new HttpClient())
            {
                var token = Session["JWT_TOKEN"]?.ToString();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{apiBase}/{model.CauHoiID}", content);

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                    ? "Cập nhật câu hỏi thành công!"
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

                var response = await client.DeleteAsync($"{apiBase}/{id}");

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode
                    ? "Xóa câu hỏi thành công!"
                    : await response.Content.ReadAsStringAsync();

                return RedirectToAction("Index");
            }
        }
    }
}
