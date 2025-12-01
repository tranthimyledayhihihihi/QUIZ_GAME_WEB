using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/quiz/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizTuyChinhController : ControllerBase
    {
        
    }
}
