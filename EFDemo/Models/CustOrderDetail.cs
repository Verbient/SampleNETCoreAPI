using System;
using System.Collections.Generic;

namespace EFDemo.Models;

public partial class CustOrderDetail
{
    public int Id { get; set; }

    public int CustOrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Amount { get; set; }

    public virtual CustOrder CustOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
