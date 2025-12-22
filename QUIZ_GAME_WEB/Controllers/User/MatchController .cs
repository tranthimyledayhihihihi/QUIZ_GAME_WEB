using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/trandau")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TranDauController : ControllerBase
    {
        private readonly IOnlineMatchService _service;
        private readonly ISocketGameServer _socketServer;

        public TranDauController(
            IOnlineMatchService service,
            ISocketGameServer socketServer)
        {
            _service = service;
            _socketServer = socketServer;
        }

        // =====================================================
        // HELPER
        // =====================================================
        private int GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(id, out var uid) ? uid : 0;
        }

       
        // =====================================================
        // ONLINE COUNT (API)
        // =====================================================
        [HttpGet("online-count")]
        [AllowAnonymous]
        public IActionResult GetOnlineCount()
        {
            int count = _socketServer.GetOnlineCount();
            return Ok(new { onlineUsers = count });
        }

      
    }
}