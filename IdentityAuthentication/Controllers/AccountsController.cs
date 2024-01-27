using IdentityAuthentication.Model;
using IdentityAuthentication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace IdentityAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService accountService;
        public AccountsController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        [HttpPost("SignUpForAdmin")]
        public async Task<IActionResult> SignUpForAdmin(SignUpModel signUpModel, string role)
        {
            var result = await accountService.Register(signUpModel, role);
            if ((int)result == 404)
                return NotFound();
            else if ((int)result == 500)
                return BadRequest();
            else if ((int)result == 200)
                return Ok(result);
            return Unauthorized();
        }
        [HttpPost("SignUpForCustomer")]
        public async Task<IActionResult> SignUpForCustomer(SignUpModel signUpModel)
        {
            var result = await accountService.Register(signUpModel, UserRoles.Customer);
            if ((int)result == 404)
                return NotFound();
            else if ((int)result == 500)
                return BadRequest();
            else if ((int)result == 200)
                return Ok(result);
            return Unauthorized();
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            var result = await accountService.SignInAsync(signInModel);
            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }
            return Ok(new { token = result });
        }
        [HttpGet("admin")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> TestForAdmin()
        {
            return Ok("This is content for Admin has authorized!!");
        }
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> TestForAll()
        {
            return Ok("This is content for everyone has authorized!!");
        }
    }
}
