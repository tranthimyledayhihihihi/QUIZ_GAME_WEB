using System.Web.Mvc;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class DashboardController : Controller
    {
        // Trang Dashboard Admin
        public ActionResult Index()
        {
            return View();  // Trang admin sẽ sử dụng layout riêng
        }
    }
}
