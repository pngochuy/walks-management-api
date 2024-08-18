using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.DTOs
{
    public class KeycloakRegisterDto
    {
        [Required]
        public string Username { get; set; } // có thể làm email cũng đc nếu là 

        [Required]
        public string Password { get; set; }
        
        [Required]
        public string PasswordConfirm { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
