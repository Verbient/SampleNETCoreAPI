using System;
using System.Collections.Generic;

namespace EFDemo.Models;

public partial class Product
{
    public int Id { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public virtual ICollection<CustOrderDetail> CustOrderDetails { get; set; } = new List<CustOrderDetail>();
}
