using System;
using System.Collections.Generic;

namespace EFDemo.Models;

public partial class CustOrder
{
    public int Id { get; set; }

    public int AppUserId { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual ICollection<CustOrderDetail> CustOrderDetails { get; set; } = new List<CustOrderDetail>();
}
