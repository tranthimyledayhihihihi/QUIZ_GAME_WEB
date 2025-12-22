using System.Web.Mvc;

namespace WEBB.Controllers.Quiz
{
    public class CustomQuizController : Controller
    {
        // GET: /Quiz/CustomQuiz/Create
        public ActionResult Create()
        {
            return View("~/Views/User/Quiz/Custom/Create.cshtml");
        }
    }
}
