using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTO;
using UserService.Services;
using UserService.Model.DTO;

namespace UserService.Controllers
{
    public class AccessController : Controller
    {
        private readonly AccessService _accessService;

        public AccessController(AccessService accessService)
        {
            _accessService = accessService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModelDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userDetailsDTO = await _accessService.AuthenticateAsync(model.Email, model.Password);
            if (userDetailsDTO != null)
            {
                return Ok(userDetailsDTO);
            }
            else
            {
                return BadRequest("Invalid credentials");
            }
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserTableModelDTO userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdUser = await _accessService.CreateUserAsync(userModel);
            if (createdUser != null)
            {
                return Ok(createdUser);
            }
            else
            {
                return BadRequest("Failed to create user. Check role details.");
            }
        }
    }
}
