
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Common;
using MyApp.Models;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

namespace MyApp.DAL
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel: class 
    {
        protected IDbConnection _dbConnection;
        private string _tableName;
        private readonly string connectionString;
        private readonly JWTAuth account;

        [ActivatorUtilitiesConstructor]
        public GenericRepository(IDbConnection dbConnection,IHttpContextAccessor httpContextAccessor)
        {
            _dbConnection = dbConnection;
            _tableName = QueryGenerator.GetTableNameFromModelName(typeof(TModel));
            this.connectionString = dbConnection.ConnectionString;
            account = (JWTAuth)httpContextAccessor.HttpContext.Items["Account"];
        }

        public IDbConnection GetDbConnection()
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.ConnectionString = connectionString;
                _dbConnection.Open();
            }
            return _dbConnection;
        }

        public virtual IEnumerable<TModel> GetAll(int? AppUserId = null, IDbConnection Connection = null!, IDbTransaction Transaction = null!)
        {
            string sql ="SELECT * FROM " + _tableName + (AppUserId == null ? "" : " WHERE AppUserId=" + AppUserId);
            IDbConnection conn;//= Connection == null ? this._dbConnection : Connection;
            if (Connection == null)
            {
                if (_dbConnection.State == ConnectionState.Closed)
                {
                    _dbConnection.ConnectionString = connectionString;
                    try
                    {
                        _dbConnection.Open();
                    }
                    catch
                    {
                        throw;
                    }
                }
                conn = _dbConnection;
            }
            else
            {
                conn = Connection;
            }
            var result = conn.Query<TModel>( sql,null,Transaction);
            _dbConnection.Dispose();
            return result;
        }

        public virtual TModel GetById(int id)
        {
            string idFieldName = null!;
            if (typeof(TModel).GetProperty(_tableName + "Id") != null)
            {
                idFieldName = _tableName + "Id"; 
            }
            else
            {
                idFieldName = "Id";
            }
            string sql = $"SELECT * FROM {_tableName} WHERE {idFieldName} = {id} " ;
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.ConnectionString = connectionString;
                _dbConnection.Open();
            }
            var result = _dbConnection.QueryFirstOrDefault<TModel>(sql);
            _dbConnection.Dispose();
            return result;
        }

        public virtual TModel Create(TModel entity, Dictionary<string, string>? FieldCustomSQL=null, IDbConnection? Connection = null, IDbTransaction? Transaction = null)
        {
            int result = 0;
            var insertQuery = QueryGenerator.GenerateInsertQuery(typeof(TModel),FieldCustomSQL,null,account?.Id);
            var ser = JsonConvert.SerializeObject(entity);
            IDbConnection conn;
            if (Connection == null)
            {
                if (_dbConnection.State == ConnectionState.Closed)
                {
                    _dbConnection.ConnectionString = connectionString;
                    _dbConnection.Open();
                }
                conn = _dbConnection;
            }
            else
            {
                conn = Connection;
            }
            try
            {   
                result = conn.ExecuteScalar<int>(insertQuery, entity, transaction: Transaction);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("UNIQUE KEY"))
                {
                    throw new CustomException("This entry is considered duplicate as similar record already exists in database");
                }
                throw;
            }

            typeof(TModel).GetProperties().Where(m => ( m.Name == "Id")).FirstOrDefault()!.SetValue(entity, result); // Populate the Id column with result
            
            return entity;
            
        }

        // Returns number of rows affected, usually 1
        public int Update(TModel entity, IDbConnection Connection = null!, IDbTransaction Transaction = null!) // Returns number of record affected TODO should return recordId
        {
            var updateQuery = QueryGenerator.GenerateUpdateQuery(typeof(TModel));
            IDbConnection connection = Connection == null ? this._dbConnection : Connection;
            
            try
            {
                //return connection.Execute(updateQuery, entity);
                var result = connection.Execute(updateQuery, entity,Transaction);
                if (Transaction == null)
                {
                    _dbConnection.Dispose();
                }
                return result;
            }
            catch 
            {
                throw;
            }
            
        }

        public int Delete(int id, IDbConnection Connection = null!, IDbTransaction Transaction = null!)
        {
            IDbConnection connection = Connection?? this._dbConnection ;
            var sql = $"{QueryGenerator.GenerateDeleteQuery(id,typeof(TModel))}";
            //return  _dbConnection.Execute(sql);
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.ConnectionString = connectionString;
                _dbConnection.Open();
            }
            var result = _dbConnection.Execute(sql,null,Transaction);
            if (Transaction == null)
            {
                _dbConnection.Dispose();
            }
            return result;
        }

        //public PagingResponse GetAllWithPagination(PagingRequest pagingRequest)
        //{
        //    return QueryGenerator.GetAllWithPagination(pagingRequest, _dbConnection, typeof(TInput));
        //}

        public void Dispose()
        {
            _dbConnection.Dispose();
        }

        public T QuerySingleOrDefault<T>(string sql)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.ConnectionString = connectionString;
                _dbConnection.Open();
            }
            var result = _dbConnection.QuerySingleOrDefault<T>(sql);
            _dbConnection.Dispose();
            return result;
        }

        public IEnumerable<T> QueryMultiple<T>(string sql)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.ConnectionString = connectionString;
                _dbConnection.Open();
            }
            var result = _dbConnection.Query<T>(sql);
            _dbConnection.Dispose();
            return result;
        }
    }
}
