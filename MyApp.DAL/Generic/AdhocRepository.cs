using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyApp.Common;

//using Serilog;

namespace MyApp.DAL
{
    public class AdhocRepository : IAdhocRepository
    {
        protected IDbConnection _dbConnection;
        private readonly ILogger logger;
        private readonly string connectionString;

        //public Repository(IOptions<DataConfig> appConnection) //: base(appConnection)
        public AdhocRepository(IDbConnection dbConnection, ILogger<AdhocRepository> logger)
        {
            connectionString = dbConnection.ConnectionString;
            _dbConnection = dbConnection;
            this.logger = logger;
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
        public int InsertMultipleRows<TModel>(IEnumerable<TModel> entities, string TableName, IDbConnection? Connection = null, IDbTransaction? Transaction = null)
        {
            
            var query = QueryGenerator.GenerateInsertQueryMultiple(entities, TableName);
            if (Connection == null)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            int affectedRows = connection.Execute(query, null, tran);
                            tran.Commit();
                            connection.Close();
                            return affectedRows;
                        }
                        catch(Exception)
                        {
                            tran.Rollback();
                            logger.LogError(query); 
                            connection.Close();
                            throw;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    int affectedRows = Connection.Execute(query, null, Transaction);
                    return affectedRows;
                }
                catch(Exception ex)
                {
                    Transaction!.Rollback();
                    logger.LogError(query);
                    Connection.Close();
                    if (ex.Message.Contains("UNIQUE KEY"))
                    {
                        throw new CustomException("This entry is considered duplicate as similar record already exists in database");
                    }
                    throw;
                }
            }
        }
        public void Dispose()
        {
            _dbConnection.Close();
        }

        //public T QuerySingleOrDefault<T>(string sql, object param = null)
        //{
        //    if (_dbConnection.State == ConnectionState.Closed)
        //    {
        //        _dbConnection.ConnectionString = connectionString;
        //        _dbConnection.Open();
        //    }
        //    return _dbConnection.QuerySingleOrDefault<T>(sql);
        //}
        public T QuerySingleOrDefault<T>(string sql, object? param = null, IDbConnection? Connection = null, IDbTransaction? Transaction = null)
        {
            if (Connection == null)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    try
                    {
                        //return connection.Execute(updateQuery, entity);
                        var result = connection.QuerySingleOrDefault<T>(sql, param);
                        _dbConnection.Dispose();
                        return result;
                    }
                    catch 
                    {
                        throw;
                    }
                }
            }
            else
            {
                var result = Connection.QuerySingleOrDefault<T>(sql, param,Transaction);
                return result;  
            }
        }

        public int ExecuteCommand(string sql, object? param = null)
        {
            try
            {
                if (_dbConnection.State == ConnectionState.Closed)
                {
                    _dbConnection.ConnectionString = connectionString;
                    _dbConnection.Open();
                }
                return _dbConnection.Execute(sql, param);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<T> Query<T>(string sql, object? param = null)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.ConnectionString = connectionString;
                _dbConnection.Open();
            }
            return _dbConnection.Query<T>(sql,param);
        }

    }
}
