using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace WEBB.Controllers.Quiz
{
    public class CustomQuizController : Controller
    {
        // GET: CustomQuiz
        public ActionResult Index()
        {
            return View("~/Views/Quiz/Custom/Index.cshtml");
        }
        // GET: CustomQuiz/Create
        public ActionResult Create()
        {
            return View("~/Views/Quiz/Custom/Create.cshtml");
        }
    }
}