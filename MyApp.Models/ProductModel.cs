using System;
using System.Collections.Generic;

namespace MyApp.Models;

public class ProductModel
{
    public int Id { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }


}
