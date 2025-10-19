using Microsoft.AspNetCore.Mvc;
using StudentADO.BusinessLogic;
using StudentADO.DTOs;
using System.Threading.Tasks;

namespace StudentADO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationLogic _authLogic;

        public AuthController(AuthenticationLogic authLogic)
        {
            _authLogic = authLogic;
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message, userId) = await _authLogic.RegisterAsync(registerDto);

            if (success)
            {
                return Ok(new { message, userId });
            }

            return BadRequest(new { message });
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message, response) = await _authLogic.LoginAsync(loginDto);

            if (success)
            {
                return Ok(response);
            }

            return Unauthorized(new { message });
        }
    }
}
