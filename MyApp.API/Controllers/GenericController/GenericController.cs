// This is a generic controller. It is the only controler which avoids the use of (generic) service layer and communicates directly with DAL layer. The use of Service layer for this controller would have been an over engineering and has been avoided.

using MyApp.Util;
using Microsoft.AspNetCore.Mvc;
using MyApp.Common;
using MyApp.DAL;
using MyApp.Models;
using System.Reflection;
using MyApp.API.Filters;
//using Microsoft.AspNetCore.Authorization;

namespace MyApp.API.Controllers
{

    /// Generic Controller 
    [Route("[controller]")]
    [ApiController]

    public class GenericController<TPostDTO, TModel> : ControllerBase where TPostDTO : class where TModel : class
    {
        Dictionary<string, string> fieldCustomSQL=null!;
        private IGenericRepository<TModel> repository;
        private JWTAuth account = null!;
        public GenericController(IGenericRepository<TModel> repository)
        {
            this.repository = repository;
            if (HttpContext != null)
            {
                account = (JWTAuth)HttpContext.Items["Account"]!;
            }
        }

        /// <summary>
        /// ModelPostPreprocessor is virtual which could be overridden in the derived class. 
        /// In this method you can:
        /// 1) Put validation logic and pre-processing on the DTO
        /// 2) Add any other calculated fields which is not part of the DTO, through fieldCustomSQL dictionary,
        /// the key is the field-Name and Value is the field-Value
        /// 3) Provide any controller specific DTO validation logics could also be provided here.
        /// </summary>
        protected virtual TModel ModelPostPreprocessor(TPostDTO DTO,TModel DtoMappedEntity, Dictionary<string, string> fieldCustomSQL = null!)
        {
            this.fieldCustomSQL = fieldCustomSQL;
            return DtoMappedEntity;
        }

        protected JWTAuth getJWTAuth()
        {
            return (JWTAuth)HttpContext.Items["Account"]!;
        }

        [Authorize("admin,superadmin")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost]
        public virtual IActionResult Post(TPostDTO dto)
        {
            TModel dataModel = (TModel)Activator.CreateInstance(typeof(TModel))!; // Create a new instance
            ObjectMapper.MapSourceObjectToTarget(dto, dataModel, true);
            dataModel = ModelPostPreprocessor(dto,dataModel);
            if (typeof(TModel).GetProperty("CreatedBy") != null)
            {
                //var account = (JWTAuth)httpContextAccessor.HttpContext.Items?["Account"];
                if (account == null)
                {
                    throw new CustomException("GenericService->Create: User not logged in");
                }
                typeof(TModel).GetProperty("CreatedBy")!.SetValue(dataModel, account.Id);
            }

            var result = repository.Create(dataModel, this.fieldCustomSQL);
            // Return a 201 Created response with the URI of the newly created resource
            return CreatedAtAction(nameof(Post), result);
        }


        /// <summary>
        /// Get an entity By Id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/{Entity}/{Id}
        /// </remarks>
        /// <param name="id"> Pass Id</param>        
        /// <response code="200">Returns the item</response>    

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("{id}")]
        public virtual IActionResult Get(int id)
        {
            return Ok(repository.GetById(id));
        }


        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            return Ok(repository.GetAll());
        }

        [Authorize("admin,superadmin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [HttpPut]
        public virtual IActionResult Put(TModel entity)
        {
            // The following code could be used to enable the override of PutPreprocessor to code for any model-preprocessing before updating, similar to PostPreprocessor
            //entity = PutPreprocessor(entity);

            var result = repository.Update(entity);
            if (result == 1)
            {
                return NoContent();
            }
            else
            {
                throw new CustomException($"{result} records are updated. The provided Id might not exist in the table {GetTableNameFromTypeName()}");
            }
        }

        /// <summary>
        /// Delete an Entity.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Delete api/Entity/{Id}
        /// </remarks>
        /// <param name="id"> Pass Generic ID</param>        
        /// <response code="204">Returns count of items deleted, usually 1 </response>    
        /// <response code="400">If there is an issue in Delete</response>  

        [Authorize("admin,superadmin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [HttpDelete("{id:int}")]
        public virtual IActionResult Delete(int id)
        {
            var result = repository.Delete(id);
            if (result == 1)
            {
                return NoContent();
            }
            else
            {
                throw new CustomException($"{result} records are deleted. The provided Id might not exist in the table {GetTableNameFromTypeName()}");
            }
        }

        private string GetTableNameFromTypeName()
        {
            string tableName = typeof(TModel).Name;
            new List<string> { "getDto", "postDto", "dto", "model" }.ForEach(m => tableName = tableName.Replace(m, "", StringComparison.InvariantCultureIgnoreCase));
            return tableName;
        }
    }
}
