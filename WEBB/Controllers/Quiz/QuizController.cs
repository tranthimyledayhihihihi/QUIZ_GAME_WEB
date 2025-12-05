using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBB.Models.Quiz;

namespace WEBB.Controllers.Quiz
{
    public class QuizController : Controller
    {
        // GET: Quiz
        public ActionResult Index()
        {
            return View(new List<CauHoiDto>());
        }
    }
}