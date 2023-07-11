using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DTO
{
    public class ProductDTO
    {
        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

    }
}
