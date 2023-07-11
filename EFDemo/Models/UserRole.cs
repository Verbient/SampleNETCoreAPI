using System;
using System.Collections.Generic;

namespace EFDemo.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
}
