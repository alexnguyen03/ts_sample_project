using Microsoft.AspNetCore.Mvc;

using ServerProject.Models;
using ServerProject.Services;

namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosController : ControllerBase
    {
        private readonly IPosService _posService = null;
        private readonly ILogger<PosController> _logger;

        public PosController(ILogger<PosController> logger, IPosService posService)
        {
            _posService = posService;
            _logger = logger;
        }
        [HttpPost("createPos")]
        public IActionResult createPos(Pos pos)
        {
            var data = _posService.Create(pos);
            return Ok(data);
        }

        [HttpPost("getAllPos")]
        public IActionResult GetAllPos()
        {
            var data = _posService.GetAll();
            return Ok(data);
        }
    }
}
