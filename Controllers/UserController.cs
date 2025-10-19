using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentADO.BusinessLogic;
using StudentADO.DTOs;
using System.Threading.Tasks;

namespace StudentADO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires JWT token for all endpoints
    public class UserController : ControllerBase
    {
        private readonly UserManagementLogic _userLogic;
        private readonly AuthenticationLogic _authLogic;

        public UserController(UserManagementLogic userLogic, AuthenticationLogic authLogic)
        {
            _userLogic = userLogic;
            _authLogic = authLogic;
        }

        // GET: api/User
        // Both Teacher and Student can view all users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userLogic.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        // Both Teacher and Student can view specific user
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userLogic.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        // POST: api/User
        // Only Teachers can create new users
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message, userId) = await _authLogic.RegisterAsync(registerDto);

            if (success)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { message, userId });
            }

            return BadRequest(new { message });
        }

        // PUT: api/User/5
        // Only Teachers can update users
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _userLogic.UpdateUserAsync(id, updateDto);

            if (success)
            {
                return Ok(new { message });
            }

            return BadRequest(new { message });
        }

        // DELETE: api/User/5
        // Only Teachers can delete users
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var (success, message) = await _userLogic.DeleteUserAsync(id);

            if (success)
            {
                return Ok(new { message });
            }

            return BadRequest(new { message });
        }
    }
}
