using System;
using System.Collections.Generic;

namespace EFDemo.Models;

public partial class AppUser
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string Email { get; set; } = null!;

    public short CountryCode { get; set; }

    public string Phone { get; set; } = null!;

    public string? Password { get; set; }

    public DateTime? PasswordChangeDateUtc { get; set; }

    public bool? IsDisabled { get; set; }

    public DateTime CreateDateTimeUtc { get; set; }

    public DateTime? LastLoginDateUtc { get; set; }

    public string? SessionId { get; set; }

    public virtual ICollection<CustOrder> CustOrders { get; set; } = new List<CustOrder>();

    public virtual UserRole? Role { get; set; }
}
