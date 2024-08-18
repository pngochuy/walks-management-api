using System.ComponentModel.DataAnnotations;

namespace NZWalks.Core.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; } // có thể làm email cũng đc nếu là 

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string[] RoleList { get; set; }
    }
}
