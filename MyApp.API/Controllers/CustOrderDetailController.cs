using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.DAL;
using MyApp.DTO;
using MyApp.Models;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustOrderDetailController : GenericController<CustOrderDetailDTO, CustOrderDetailModel>
    {
        public CustOrderDetailController(IGenericRepository<CustOrderDetailModel> genericRepository) : base(genericRepository) { }
    }
}
