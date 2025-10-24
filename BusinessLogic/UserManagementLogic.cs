using StudentADO.DTOs;
using StudentADO.Models;
using StudentADO.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentADO.BusinessLogic
{
    public class UserManagementLogic
    {
        private readonly IUserRepository _userRepository;

        public UserManagementLogic(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Get all users
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => MapToUserDTO(u)).ToList();
        }

        // Get user by ID
        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? MapToUserDTO(user) : null;
        }

        // Update user - FIXED VERSION
        public async Task<(bool success, string message)> UpdateUserAsync(int userId, UpdateUserDTO updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return (false, "User not found");
            }

            // Check if email is being changed and already exists
            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
                if (existingUser != null && existingUser.UserId != userId)
                {
                    return (false, "Email already exists");
                }
                user.Email = updateDto.Email;
            }

            // Update all provided fields
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                user.Name = updateDto.Name;
            }

            if (!string.IsNullOrEmpty(updateDto.Designation))
            {
                user.Designation = updateDto.Designation;
            }

            if (updateDto.DateOfBirth.HasValue)
            {
                user.DateOfBirth = updateDto.DateOfBirth.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;

            bool updated = await _userRepository.UpdateAsync(user);

            if (updated)
            {
                return (true, "User updated successfully");
            }

            return (false, "Update failed");
        }

        // Delete user
        public async Task<(bool success, string message)> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return (false, "User not found");
            }

            bool deleted = await _userRepository.DeleteAsync(userId);

            if (deleted)
            {
                return (true, "User deleted successfully");
            }

            return (false, "Delete failed");
        }

        // Helper method to map User to UserDTO
        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Designation = user.Designation,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
