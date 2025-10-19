using StudentADO.DTOs;
using StudentADO.Helpers;
using StudentADO.Models;
using StudentADO.Repositories;
using System;
using System.Threading.Tasks;

namespace StudentADO.BusinessLogic
{
    public class AuthenticationLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthenticationLogic(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        // Register new user
        public async Task<(bool success, string message, int userId)> RegisterAsync(RegisterDTO registerDto)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                return (false, "Email already exists", 0);
            }

            // Validate designation
            if (registerDto.Designation != "Teacher" && registerDto.Designation != "Student")
            {
                return (false, "Designation must be either 'Teacher' or 'Student'", 0);
            }

            // Create user
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = PasswordHelper.HashPassword(registerDto.Password),
                DateOfBirth = registerDto.DateOfBirth,
                Designation = registerDto.Designation,
                CreatedAt = DateTime.UtcNow
            };

            int userId = await _userRepository.CreateAsync(user);

            if (userId > 0)
            {
                return (true, "Registration successful", userId);
            }

            return (false, "Registration failed", 0);
        }

        // Login user
        public async Task<(bool success, string message, LoginResponseDTO response)> LoginAsync(LoginDTO loginDto)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return (false, "Invalid email or password", null);
            }

            // Verify password
            if (!PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return (false, "Invalid email or password", null);
            }

            // Generate JWT token
            string token = _jwtHelper.GenerateToken(user.UserId, user.Email, user.Designation);

            var response = new LoginResponseDTO
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Designation = user.Designation
            };

            return (true, "Login successful", response);
        }
    }
}
