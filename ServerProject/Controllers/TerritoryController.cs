using Microsoft.AspNetCore.Mvc;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerritoryController : ControllerBase
    {
        private readonly ITerritoryService _territoryService = null;
        public TerritoryController(ITerritoryService territoryService)
        {
            this._territoryService = territoryService;
        }
        [HttpGet]
        [Route("getTerritories")]
        public IActionResult getTerritories()
        {
            var data = _territoryService.GetAll();
            return Ok(data);
        }
    }
}
