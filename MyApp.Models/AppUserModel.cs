using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class AppUserModel
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public short CountryCode { get; set; }

    public string Phone { get; set; } = null!;

    public string? Password { get; set; }

}
