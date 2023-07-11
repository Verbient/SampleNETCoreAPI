using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DTO
{
    public class EmailDTO
    {
        [EmailAddress]
        [Required(ErrorMessage = "{0} is required")]
        [MaxLength(40, ErrorMessage = "{0}: Only {1} characters are allowed")]
        public string Email { get; set; } = null!;
    }
}
