using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



namespace MyApp.DAL
{
    public interface IGenericRepository<TModel> : IDisposable where TModel: class
    {
        public IDbConnection GetDbConnection();
        IEnumerable<TModel> GetAll(int? AppUserId = null, IDbConnection Connection = null!, IDbTransaction Transaction = null!);


        TModel GetById(int id);

        TModel Create(TModel entity, Dictionary<string, string> FieldCustomSQL=null!, IDbConnection Connection = null!, IDbTransaction Transaction = null!);
        int Update(TModel entity, IDbConnection Connection = null!, IDbTransaction Transaction = null!);

        int Delete(int id, IDbConnection Connection = null!, IDbTransaction Transaction = null!);

        T QuerySingleOrDefault<T>(string sql);

        IEnumerable<T> QueryMultiple<T>(string sql);
        //Task<TInput> GetByConditionAsync(Expression<Func<TInput, bool>> expression);

        //PagingResponse GetAllWithPagination(PagingRequest paging);
        //PagingResponse GetAllWithPagination(string spName, int pageDraw, DynamicParameters dynamicParameters = null);
        //int GetOffset(int pageSize, int pageNumber);
        
    }
}
