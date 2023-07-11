
using static MyApp.Common.Enums;

namespace MyApp.Models
{
    public class JWTAuth
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string Phone{ get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public UserRoles RoleName { get; set; }

        //public PaymentProviders rRoleName { get; set; }
    }
}
