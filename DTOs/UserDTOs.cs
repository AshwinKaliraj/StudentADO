using System;
using System.ComponentModel.DataAnnotations;

namespace StudentADO.DTOs
{
    // Get User Response
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Designation { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Update User Request
    public class UpdateUserDTO
    {
        [StringLength(100)]
        public string Name { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
