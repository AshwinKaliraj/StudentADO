using System;
using System.ComponentModel.DataAnnotations;

namespace StudentADO.DTOs
{
    // Login Request
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    // Register Request
    public class RegisterDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression("^(Teacher|Student)$", ErrorMessage = "Designation must be either 'Teacher' or 'Student'")]
        public string Designation { get; set; }
    }

    // Login Response
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
    }
}
