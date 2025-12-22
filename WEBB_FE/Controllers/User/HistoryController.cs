using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WEBB.Models.ViewModels;
namespace WEBB.Controllers.User
{
    public class HistoryController : Controller
    {
        private readonly string _apiBase = "https://localhost:7092/";
        public async Task<ActionResult> Index(int page = 1)
        {
            var token = Session["JWT_TOKEN"] as string;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // API Backend: api/LichSuChoi/my?pageNumber=X&pageSize=Y
                var res = await client.GetAsync($"api/LichSuChoi/my?pageNumber={page}&pageSize=10");

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();

                    // Vì Newtonsoft.Json mặc định không phân biệt hoa thường nên nó sẽ tự map
                    // tongSoKetQua -> TongSoKetQua
                    var data = JsonConvert.DeserializeObject<HistoryListViewModel>(json);

                    return View("~/Views/User/History/Index.cshtml", data);
                }
            }
            // Nếu lỗi hoặc không có dữ liệu, trả về model rỗng để không lỗi View
            return View("~/Views/User/History/Index.cshtml", new HistoryListViewModel());
        }
    }
}