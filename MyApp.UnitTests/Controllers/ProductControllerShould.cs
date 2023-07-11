
using MyApp.DTO;
using MyApp.Models;
using MyApp.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MyApp.Tests.Controllers
{
    public class ProductControllerShould:TControllerShould<ProductDTO, ProductModel>
    {
        public ProductControllerShould(ITestOutputHelper output):base(output,"ProductName")
        {
            
        }

        [Fact, TestPriority(1)]
        public override void GetAll()
        {
            base.GetAll();
        }

        [Fact, TestPriority(2)]
        public override void Get()
        {
            base.Get();
        }

        [Fact, TestPriority(3)]
        public override void  Post_Put_Delete()
        {
            base.Post_Put_Delete();
        }

    }
}
