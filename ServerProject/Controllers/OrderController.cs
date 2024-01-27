using Microsoft.AspNetCore.Mvc;
using ServerProject.Models;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService = null;
        public OrderController(IOrderService orderService)
        {
            this._orderService = orderService;
        }
        [HttpGet]
        [Route("getOrders")]
        public IActionResult getAllOrder()
        {
            var data = _orderService.GetOrders();
            return Ok(data);
        }
        [HttpGet]
        [Route("getOrder")]
        public IActionResult getAllOrderById(int orderId)
        {
            var data = _orderService.GetAllOrdersByOrderId(orderId);
            return Ok(data);
        }
        [HttpPost]
        [Route("createOrders")]
        public IActionResult createOrder(Order order)
        {
            var data = _orderService.Create(order);
            return Ok(data);
        }
        [HttpDelete]
        [Route("deleteOrder")]
        public IActionResult deleteOrder(int orderId)
        {
            try
            {
                var data = _orderService.Delete(orderId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); ;
                return NotFound();
            }
        }
        [HttpPut]
        [Route("updateOrder")]
        public IActionResult updateOrder(Order order)
        {
            var data = _orderService.Update(order);
            return Ok(data);
        }
    }
}
