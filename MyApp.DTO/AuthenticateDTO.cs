using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DTO
{
    // Used for returning result wuth JWT after successful Authentication
    public class AuthenticateDTO
    {
        public int AppUserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int CountryCode { get; set; }
        public string RoleName { get; set; } = null!;
        public string JwtToken { get; set; } = null!;
    }
}
