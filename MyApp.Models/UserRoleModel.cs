using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class UserRoleModel
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }
}
