using Microsoft.AspNetCore.Mvc;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService = null;
        public SupplierController(ISupplierService categoryService)
        {
            this._supplierService = categoryService;
        }
        [HttpGet]
        [Route("getSuppliers")]
        public IActionResult getSuppliers()
        {
            var data = _supplierService.GetAll();
            return Ok(data);
        }
    }
}
