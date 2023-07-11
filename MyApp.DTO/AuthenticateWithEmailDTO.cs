using System.ComponentModel.DataAnnotations;

namespace MyApp.DTO
{
    public class AuthenticateWithEmailDTO:EmailDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; } = null!;
    }
}
