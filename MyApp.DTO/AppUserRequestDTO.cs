using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyApp.DTO
{
    public class AppUserRequestDTO
    {
        [MaxLength(30, ErrorMessage = "Only {1} characters are allowed for {0}")]
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(30, ErrorMessage = "Only {1} characters are allowed for {0}")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 300,ErrorMessage="{0} should be between {1} and {2}")]
        public short CountryCode { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Phone { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
