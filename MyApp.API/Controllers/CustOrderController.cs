using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.DAL;
using MyApp.DTO;
using MyApp.Models;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustOrderController : GenericController<CustOrderDTO, CustOrderModel>
    {
        public CustOrderController(IGenericRepository<CustOrderModel> genericRepository) : base(genericRepository) { }

    }
}
