using Microsoft.AspNetCore.Mvc;
using ServerProject.Models;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperController : ControllerBase
    {
        private readonly IShipperService _shipperService = null;
        public ShipperController(IShipperService shipperService)
        {
            this._shipperService = shipperService;
        }
        [HttpGet]
        [Route("getShippers")]
        public IActionResult getShippers()
        {
            var data = _shipperService.GetAll();
            return Ok(data);
        }
        [HttpPost]
        [Route("createShipper")]
        public IActionResult createShipper(Shipper shipper)
        {
            var data = _shipperService.Create(shipper);
            return Ok(data);
        }
        [HttpDelete]
        [Route("deleteShipper")]
        public IActionResult deleteShipper(int shipperId)
        {
            var data = _shipperService.Delete(shipperId);
            return Ok(data);
        }
        [HttpPut]
        [Route("updateShipper")]
        public IActionResult updateShipper(Shipper shipper)
        {
            var data = _shipperService.Update(shipper);
            return Ok(data);
        }
    }
}
