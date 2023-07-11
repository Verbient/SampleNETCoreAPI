using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class CustOrderDetailModel
{
    public int Id { get; set; }

    public int CustOrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Amount { get; set; }

}
