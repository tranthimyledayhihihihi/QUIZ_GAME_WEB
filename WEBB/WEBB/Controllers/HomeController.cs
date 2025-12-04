using System.Web.Mvc;

namespace WEBB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(); // sẽ tìm Views/Home/Index.cshtml
        }
    }
}
