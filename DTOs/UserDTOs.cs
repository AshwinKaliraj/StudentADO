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
    namespace StudentADO.DTOs
    {
        public class UpdateUserDTO
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Designation { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }
    }

}
