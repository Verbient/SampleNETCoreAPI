using System.Collections.Generic;
using System.Data;

namespace MyApp.DAL
{
    public interface IAdhocRepository
    {
        public IDbConnection GetDbConnection();
        int InsertMultipleRows<TModel>(IEnumerable<TModel> entities, string TableName, IDbConnection Connection = null!, IDbTransaction Transaction = null!);
        public T QuerySingleOrDefault<T>(string sql, object param = null!, IDbConnection Connection = null!, IDbTransaction Transaction = null!);
        int ExecuteCommand(string sql, object param = null!);
        IEnumerable<T> Query<T>(string sql, object param = null!);
        void Dispose();
    }
}